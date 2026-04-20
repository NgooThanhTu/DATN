using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManagement.Application.DTOs.AI;
using TaskManagement.Application.DTOs.WorkTask;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class GeminiAiService : IAiService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IWorkTaskService _workTaskService;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public GeminiAiService(
            ApplicationDbContext context,
            HttpClient httpClient,
            IWorkTaskService workTaskService,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _workTaskService = workTaskService;
            _configuration = configuration;
        }

        public async Task<string> ChatAsync(Guid userId, AiChatRequestDto request)
        {
            await EnsureQuotaAsync(userId);

            var history = request.History?
                .Where(m => !string.IsNullOrWhiteSpace(m.Content))
                .TakeLast(10)
                .Select(m => $"{m.Role}: {m.Content}")
                .ToList() ?? new List<string>();

            var prompt = $"""
            You are SprintA AI, a concise Vietnamese project-management assistant.
            Help with tasks, projects, cycles, modules, Agile planning, summaries, and risk analysis.
            Do not invent private data. If project context is missing, ask for the missing detail.

            Recent conversation:
            {string.Join("\n", history)}

            User message:
            {request.Message}
            """;

            var result = await GenerateTextAsync(userId, "ai-chat", prompt);
            return string.IsNullOrWhiteSpace(result.Text)
                ? "Mình chưa tạo được phản hồi từ AI. Hãy kiểm tra API key Gemini hoặc thử lại sau."
                : result.Text.Trim();
        }

        public async Task<string> GenerateDescriptionAsync(Guid userId, AiGenerateDescriptionRequestDto request)
        {
            await EnsureQuotaAsync(userId);

            var prompt = $"""
            You are SprintA AI. Write a clear work item description in Vietnamese Markdown.
            Return only the description, no surrounding explanation.

            Work item prompt:
            {request.Prompt}

            Optional context:
            {request.Context}
            """;

            var result = await GenerateTextAsync(userId, "generate-description", prompt);
            return result.Text.Trim();
        }

        public async Task<List<AiSubTaskDto>> BreakdownTaskAsync(Guid userId, AiBreakdownRequestDto request)
        {
            await EnsureQuotaAsync(userId);

            var prompt = $$"""
            You are SprintA AI. Break the parent work item into practical sub-work items.
            Return STRICT JSON only, matching this schema:
            [
              {
                "title": "short actionable title",
                "description": "implementation detail",
                "estHours": 2,
                "priority": 3
              }
            ]
            Rules:
            - Return 3 to 7 subtasks.
            - priority is an integer: 1 urgent, 2 high, 3 medium, 4 low.
            - estHours is a number from 0.5 to 16.
            - No markdown, no code fences, no prose outside JSON.

            Parent title: {{request.Title}}
            Parent description: {{request.Description}}
            """;

            var result = await GenerateTextAsync(userId, "breakdown-task", prompt, forceJson: true);
            return DeserializeSubtasks(result.Text);
        }

        public async Task<List<WorkTaskResponseDto>> BreakdownAndCreateSubtasksAsync(Guid userId, AiBreakdownRequestDto request)
        {
            if (request.ProjectId == Guid.Empty)
            {
                throw new ArgumentException("ProjectId la bat buoc khi tao sub-work items bang AI.");
            }

            if (!request.ParentTaskId.HasValue)
            {
                throw new ArgumentException("ParentTaskId la bat buoc khi tao sub-work items bang AI.");
            }

            var subtasks = await BreakdownTaskAsync(userId, request);
            var created = new List<WorkTaskResponseDto>();

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                created.Clear();
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    foreach (var subtask in subtasks)
                    {
                        var dto = new CreateWorkTaskDto
                        {
                            ProjectId = request.ProjectId,
                            ParentTaskId = request.ParentTaskId,
                            Title = subtask.Title,
                            Description = subtask.Description,
                            StatusName = "TO DO",
                            Priority = subtask.Priority is >= 1 and <= 4 ? subtask.Priority : 3,
                            TotalEstimatedHours = Math.Max(0, subtask.EstHours)
                        };

                        created.Add(await _workTaskService.CreateAsync(userId, dto));
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return created;
        }

        public async Task<AiUsageDto> GetUsageAsync(Guid userId)
        {
            var quota = GetMonthlyQuota();
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var used = await _context.AITokenUsages
                .Where(x => x.UserId == userId && x.CreatedAt >= monthStart)
                .SumAsync(x => (long?)x.TokensUsed) ?? 0;

            return new AiUsageDto
            {
                UsedTokensThisMonth = used,
                MonthlyTokenQuota = quota
            };
        }

        private async Task EnsureQuotaAsync(Guid userId)
        {
            var usage = await GetUsageAsync(userId);
            if (usage.UsedTokensThisMonth >= usage.MonthlyTokenQuota)
            {
                throw new InvalidOperationException($"Ban da vuot han muc AI thang nay ({usage.MonthlyTokenQuota:N0} tokens).");
            }
        }

        private async Task<GeminiResult> GenerateTextAsync(Guid userId, string featureCode, string prompt, bool forceJson = false)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("PASTE_YOUR_GEMINI_API_KEY_HERE", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Chua cau hinh Gemini API key. Hay nhap key vao appsettings.json tai Gemini:ApiKey.");
            }

            var model = _configuration["Gemini:Model"] ?? "gemini-1.5-flash";
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={Uri.EscapeDataString(apiKey)}";

            var payload = new
            {
                systemInstruction = new
                {
                    parts = new[] { new { text = "You must follow the user requested output format exactly." } }
                },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = prompt } }
                    }
                },
                generationConfig = new
                {
                    temperature = forceJson ? 0.2 : 0.5,
                    responseMimeType = forceJson ? "application/json" : "text/plain"
                }
            };

            using var response = await _httpClient.PostAsJsonAsync(endpoint, payload, _jsonOptions);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Gemini API loi {(int)response.StatusCode}: {responseBody}");
            }

            var result = ParseGeminiResponse(responseBody);
            var fallbackTokenEstimate = Math.Max(1, (prompt.Length + result.Text.Length) / 4);

            _context.AITokenUsages.Add(new AITokenUsage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FeatureCode = featureCode,
                TokensUsed = result.TotalTokens > 0 ? result.TotalTokens : fallbackTokenEstimate,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return result;
        }

        private GeminiResult ParseGeminiResponse(string responseBody)
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            var text = "";
            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var first = candidates[0];
                if (first.TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts))
                {
                    text = string.Join("", parts.EnumerateArray()
                        .Where(part => part.TryGetProperty("text", out _))
                        .Select(part => part.GetProperty("text").GetString()));
                }
            }

            long totalTokens = 0;
            if (root.TryGetProperty("usageMetadata", out var usage) &&
                usage.TryGetProperty("totalTokenCount", out var totalTokenCount))
            {
                totalTokens = totalTokenCount.GetInt64();
            }

            return new GeminiResult(text ?? "", totalTokens);
        }

        private List<AiSubTaskDto> DeserializeSubtasks(string rawText)
        {
            var json = rawText.Trim();
            if (json.StartsWith("```", StringComparison.Ordinal))
            {
                json = json.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "", StringComparison.Ordinal)
                    .Trim();
            }

            var subtasks = JsonSerializer.Deserialize<List<AiSubTaskDto>>(json, _jsonOptions);
            if (subtasks == null || subtasks.Count == 0)
            {
                throw new InvalidOperationException("AI khong tra ve danh sach subtask hop le.");
            }

            return subtasks
                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                .Take(10)
                .ToList();
        }

        private long GetMonthlyQuota()
        {
            var value = _configuration["Gemini:MonthlyTokenQuota"];
            return long.TryParse(value, out var quota) && quota > 0 ? quota : 100_000;
        }

        private sealed record GeminiResult(string Text, long TotalTokens);
    }
}
