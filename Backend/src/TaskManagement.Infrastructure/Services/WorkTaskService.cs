using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.WorkTask;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly ApplicationDbContext _context;

        public WorkTaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkTaskResponseDto> CreateAsync(Guid reporterId, CreateWorkTaskDto request)
        {
            // Optional: Add logic to validate Project, Sprint, TaskType existing etc.
            var taskStatus = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.Name.Contains("To Do") || ts.Name.Contains("Cần làm"));
            if (taskStatus == null) throw new InvalidOperationException("Default task status not found.");

            var workTask = new TaskManagement.Domain.Entities.WorkTask
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                SprintId = request.SprintId,
                ParentTaskId = request.ParentTaskId,
                TaskTypeId = request.TaskTypeId,
                TaskStatusId = taskStatus.Id,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                StoryPoints = request.StoryPoints,
                PlannedStartDate = request.PlannedStartDate,
                PlannedEndDate = request.PlannedEndDate,
                ReporterId = reporterId,
                AssignedUserId = request.AssignedUserId,
                DueDate = request.DueDate,
                TotalEstimatedHours = request.TotalEstimatedHours,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.WorkTasks.Add(workTask);
            await _context.SaveChangesAsync();

            return new WorkTaskResponseDto
            {
                Id = workTask.Id,
                Title = workTask.Title,
                Description = workTask.Description,
                ProjectId = workTask.ProjectId,
                SprintId = workTask.SprintId,
                TaskTypeId = workTask.TaskTypeId,
                TaskStatusId = workTask.TaskStatusId,
                ReporterId = workTask.ReporterId,
                AssignedUserId = workTask.AssignedUserId,
                CreatedAt = workTask.CreatedAt,
                UpdatedAt = workTask.UpdatedAt
            };
        }

        public async Task UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequestDto request)
        {
            var taskToUpdate = await _context.WorkTasks
                .Include(wt => wt.TaskStatus)
                .Include(wt => wt.Sprint) // Load Sprint để check Sprint Lock
                .FirstOrDefaultAsync(wt => wt.Id == taskId && !wt.IsDeleted);

            if (taskToUpdate == null) throw new ArgumentException("Tác vụ không tồn tại.");

            // === 5.4 SPRINT LOCK: Chặn chỉnh sửa Task của Sprint đã đóng ===
            if (taskToUpdate.Sprint != null && !taskToUpdate.Sprint.Status)
            {
                throw new InvalidOperationException("Không thể chỉnh sửa Task của một Sprint đã đóng.");
            }

            var oldStatus = taskToUpdate.TaskStatus;
            var newStatus = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.Id == request.TaskStatusId);

            if (newStatus == null) throw new ArgumentException("Trạng thái chuyển đến không tồn tại.");

            if (oldStatus.Id == newStatus.Id) return;

            // 1. State Machine Guardrails: Prevent skipping statuses
            if (Math.Abs(oldStatus.Position - newStatus.Position) > 1)
            {
                throw new InvalidOperationException("Không được phép nhảy cóc trạng thái. Vui lòng chuyển thẻ theo thứ tự.");
            }

            bool isMovingToActiveOrDone = newStatus.Name.Contains("In Progress", StringComparison.OrdinalIgnoreCase) ||
                                          newStatus.Name.Contains("Done", StringComparison.OrdinalIgnoreCase) ||
                                          newStatus.Name.Contains("Đang làm", StringComparison.OrdinalIgnoreCase) ||
                                          newStatus.Name.Contains("Hoàn thành", StringComparison.OrdinalIgnoreCase);

            // 2. Task Dependencies / Blockers
            if (isMovingToActiveOrDone)
            {
                var blockers = await _context.TaskDependencies
                    .Include(td => td.PredecessorTask)
                    .ThenInclude(pt => pt.TaskStatus)
                    .Where(td => td.SuccessorTaskId == taskId)
                    .Select(td => td.PredecessorTask)
                    .ToListAsync();

                foreach (var blocker in blockers)
                {
                    bool isBlockerDone = blocker.TaskStatus.Name.Contains("Done", StringComparison.OrdinalIgnoreCase) ||
                                         blocker.TaskStatus.Name.Contains("Hoàn thành", StringComparison.OrdinalIgnoreCase);
                    if (!isBlockerDone)
                    {
                        throw new InvalidOperationException($"Tác vụ phụ thuộc '{blocker.Title}' chưa hoàn thành. Không thể chuyển trạng thái.");
                    }
                }
            }

            // 3. Parent-Subtask Constraint
            bool isMovingToDone = newStatus.Name.Contains("Done", StringComparison.OrdinalIgnoreCase) ||
                                  newStatus.Name.Contains("Hoàn thành", StringComparison.OrdinalIgnoreCase);

            if (isMovingToDone)
            {
                bool hasUnfinishedSubtasks = await _context.WorkTasks
                    .Include(wt => wt.TaskStatus)
                    .AnyAsync(wt => wt.ParentTaskId == taskId && !wt.IsDeleted &&
                                    !wt.TaskStatus.Name.Contains("Done", StringComparison.OrdinalIgnoreCase) &&
                                    !wt.TaskStatus.Name.Contains("Hoàn thành", StringComparison.OrdinalIgnoreCase));

                if (hasUnfinishedSubtasks)
                {
                    throw new InvalidOperationException("Không thể hoàn thành tác vụ cha khi vẫn còn tác vụ con (subtask) chưa hoàn thành.");
                }
            }

            // Apply modifications
            taskToUpdate.TaskStatusId = newStatus.Id;
            taskToUpdate.UpdatedAt = DateTime.UtcNow; // UTC enforcement for UpdatedAt

            // Apply RowVersion provided from client for Concurrency Check
            if (request.RowVersion != null && request.RowVersion.Length > 0)
            {
                _context.Entry(taskToUpdate).Property(nameof(taskToUpdate.RowVersion)).OriginalValue = request.RowVersion;
            }

            await _context.SaveChangesAsync();
        }
    }
}
