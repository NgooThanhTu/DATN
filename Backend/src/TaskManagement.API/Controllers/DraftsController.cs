using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DraftsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DraftsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User not found in token");
            return Guid.Parse(userIdString);
        }

        [HttpGet]
        public async Task<IActionResult> GetDrafts()
        {
            var userId = GetUserId();
            var drafts = await _context.TaskDrafts
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.UpdatedAt)
                .ToListAsync();

            // Parse PayloadJson and merge into response so frontend gets statusName, priority etc.
            var result = drafts.Select(d =>
            {
                var obj = new Dictionary<string, object?>
                {
                    { "id", d.Id },
                    { "userId", d.UserId },
                    { "title", d.Title },
                    { "description", d.Description },
                    { "createdAt", d.CreatedAt },
                    { "updatedAt", d.UpdatedAt }
                };

                if (!string.IsNullOrEmpty(d.PayloadJson))
                {
                    try
                    {
                        var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(d.PayloadJson);
                        if (payload != null)
                        {
                            foreach (var kv in payload)
                            {
                                if (!obj.ContainsKey(kv.Key))
                                {
                                    obj[kv.Key] = kv.Value.ValueKind switch
                                    {
                                        JsonValueKind.String => kv.Value.GetString(),
                                        JsonValueKind.Number => kv.Value.GetInt32(),
                                        JsonValueKind.True => true,
                                        JsonValueKind.False => false,
                                        _ => kv.Value.ToString()
                                    };
                                }
                            }
                        }
                    }
                    catch { /* Ignore parse errors */ }
                }

                // Ensure defaults
                if (!obj.ContainsKey("statusName")) obj["statusName"] = "BACKLOG";
                if (!obj.ContainsKey("priority")) obj["priority"] = 3;

                return obj;
            }).ToList();

            return Ok(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDraft([FromBody] DraftCreateUpdateDto dto)
        {
            var userId = GetUserId();

            // Store statusName and priority in PayloadJson
            var payloadObj = new Dictionary<string, object?>
            {
                { "statusName", dto.StatusName ?? "BACKLOG" },
                { "priority", dto.Priority ?? 3 }
            };

            var draft = new TaskDraft
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                PayloadJson = JsonSerializer.Serialize(payloadObj),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TaskDrafts.Add(draft);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                data = new
                {
                    draft.Id,
                    draft.UserId,
                    draft.Title,
                    draft.Description,
                    statusName = dto.StatusName ?? "BACKLOG",
                    priority = dto.Priority ?? 3,
                    draft.CreatedAt,
                    draft.UpdatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDraft(Guid id, [FromBody] DraftCreateUpdateDto dto)
        {
            var userId = GetUserId();
            var draft = await _context.TaskDrafts.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (draft == null)
                return NotFound(new { message = "Draft not found" });

            draft.Title = dto.Title ?? draft.Title;
            draft.Description = dto.Description ?? draft.Description;

            // Parse existing payload and merge updates
            var payloadObj = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(draft.PayloadJson))
            {
                try
                {
                    var existing = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(draft.PayloadJson);
                    if (existing != null)
                    {
                        foreach (var kv in existing)
                        {
                            payloadObj[kv.Key] = kv.Value.ValueKind switch
                            {
                                JsonValueKind.String => kv.Value.GetString(),
                                JsonValueKind.Number => kv.Value.GetInt32(),
                                _ => kv.Value.ToString()
                            };
                        }
                    }
                }
                catch { }
            }

            if (dto.StatusName != null) payloadObj["statusName"] = dto.StatusName;
            if (dto.Priority.HasValue) payloadObj["priority"] = dto.Priority.Value;

            draft.PayloadJson = JsonSerializer.Serialize(payloadObj);
            draft.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                data = new
                {
                    draft.Id,
                    draft.UserId,
                    draft.Title,
                    draft.Description,
                    statusName = payloadObj.ContainsKey("statusName") ? payloadObj["statusName"] : "BACKLOG",
                    priority = payloadObj.ContainsKey("priority") ? payloadObj["priority"] : 3,
                    draft.CreatedAt,
                    draft.UpdatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDraft(Guid id)
        {
            var userId = GetUserId();
            var draft = await _context.TaskDrafts.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            
            if (draft == null)
            {
                return NotFound();
            }

            _context.TaskDrafts.Remove(draft);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Draft deleted" });
        }
    }

    public class DraftCreateUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? StatusName { get; set; }
        public int? Priority { get; set; }
    }
}
