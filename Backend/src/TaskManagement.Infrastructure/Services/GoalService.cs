using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context;

        public GoalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetAllAsync(Guid workspaceId)
        {
            var goals = await _context.Goals
                .AsNoTracking()
                .Include(g => g.Owner)
                .Where(g => g.WorkspaceId == workspaceId && !g.IsArchived)
                .ToListAsync();

            return goals.Select(g => new {
                id = g.Id,
                title = g.Title,
                status = g.Status,
                progress = g.Progress,
                dueDate = g.DueDate,
                createdAt = g.CreatedAt,
                ownerId = g.OwnerId,
                owner = g.Owner != null ? new {
                    id = g.Owner.Id,
                    fullName = g.Owner.FullName,
                    email = g.Owner.Email,
                    avatarUrl = g.Owner.AvatarUrl
                } : null
            });
        }

        public async Task<object?> GetByIdAsync(Guid id)
        {
            var g = await _context.Goals
                .AsNoTracking()
                .Include(g => g.Owner)
                .Include(g => g.Updates)
                .Include(g => g.Lessons)
                .Include(g => g.Risks)
                .Include(g => g.Decisions)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (g == null) return null;

            return new {
                id = g.Id,
                title = g.Title,
                description = g.Description,
                status = g.Status,
                progress = g.Progress,
                dueDate = g.DueDate,
                createdAt = g.CreatedAt,
                ownerId = g.OwnerId,
                owner = g.Owner != null ? new {
                    id = g.Owner.Id,
                    fullName = g.Owner.FullName,
                    email = g.Owner.Email,
                    avatarUrl = g.Owner.AvatarUrl
                } : null,
                updates = g.Updates,
                lessons = g.Lessons,
                risks = g.Risks,
                decisions = g.Decisions
            };
        }

        public async Task<object> CreateAsync(Guid creatorId, Guid workspaceId, object dto)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(System.Text.Json.JsonSerializer.Serialize(dto));
            
            var title = data.TryGetProperty("title", out var t) ? t.GetString() : null;
            var status = data.TryGetProperty("status", out var s) ? s.GetString() : null;

            var ownerId = creatorId;
            if (data.TryGetProperty("ownerId", out var oid) && Guid.TryParse(oid.GetString(), out var parsedOwnerId))
            {
                ownerId = parsedOwnerId;
            }

            var goal = new Goal
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                WorkspaceId = workspaceId,
                Title = string.IsNullOrEmpty(title) ? "New Goal" : title,
                Status = string.IsNullOrEmpty(status) ? "ĐANG CHỜ XỬ LÝ" : status,
                CreatedAt = DateTime.UtcNow
            };
            
            if (data.TryGetProperty("date", out var dateObj))
            {
                var dateStr = dateObj.GetString();
                if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var d))
                {
                    goal.DueDate = d;
                }
            }

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task<object> UpdateAsync(Guid id, object dto)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                goal.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return goal;
        }

        public async Task ArchiveAsync(Guid id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                goal.IsArchived = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                _context.Goals.Remove(goal);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<object> AddUpdateAsync(Guid goalId, Guid userId, object dto)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(System.Text.Json.JsonSerializer.Serialize(dto));
            var update = new GoalUpdate
            {
                Id = Guid.NewGuid(),
                GoalId = goalId,
                UserId = userId,
                Content = data.TryGetProperty("content", out var content) ? content.GetString() ?? "" : "",
                Status = data.TryGetProperty("status", out var status) ? status.GetString() ?? "" : "",
                CreatedAt = DateTime.UtcNow
            };
            _context.GoalUpdates.Add(update);
            await _context.SaveChangesAsync();
            return update;
        }

        public async Task<object> AddLessonAsync(Guid goalId, Guid userId, object dto)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(System.Text.Json.JsonSerializer.Serialize(dto));
            var lesson = new GoalLesson
            {
                Id = Guid.NewGuid(),
                GoalId = goalId,
                CreatorId = userId,
                Text = data.TryGetProperty("text", out var text) ? text.GetString() ?? "" : "",
                CreatedAt = DateTime.UtcNow
            };
            _context.GoalLessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<object> AddRiskAsync(Guid goalId, Guid userId, object dto)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(System.Text.Json.JsonSerializer.Serialize(dto));
            var risk = new GoalRisk
            {
                Id = Guid.NewGuid(),
                GoalId = goalId,
                CreatorId = userId,
                Text = data.TryGetProperty("text", out var text) ? text.GetString() ?? "" : "",
                Severity = data.TryGetProperty("severity", out var severity) ? severity.GetString() ?? "" : "",
                CreatedAt = DateTime.UtcNow
            };
            _context.GoalRisks.Add(risk);
            await _context.SaveChangesAsync();
            return risk;
        }

        public async Task<object> AddDecisionAsync(Guid goalId, Guid userId, object dto)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(System.Text.Json.JsonSerializer.Serialize(dto));
            var decision = new GoalDecision
            {
                Id = Guid.NewGuid(),
                GoalId = goalId,
                CreatorId = userId,
                Text = data.TryGetProperty("text", out var text) ? text.GetString() ?? "" : "",
                CreatedAt = DateTime.UtcNow
            };
            _context.GoalDecisions.Add(decision);
            await _context.SaveChangesAsync();
            return decision;
        }
    }
}
