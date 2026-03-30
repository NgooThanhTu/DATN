using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        // Roles that can see ALL tasks in a project
        private static readonly string[] ManagerRoles = { "PM", "PO", "SM", "PROJECT_MANAGER", "SCRUM_MASTER" };

        public WorkTaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkTaskResponseDto>> GetByProjectAsync(Guid projectId, Guid userId)
        {
            // 1. Check user is a member of this project
            var membership = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException("Bạn không phải thành viên của dự án này.");
            }

            // 2. Load all tasks for this project
            var query = _context.WorkTasks
                .Include(wt => wt.TaskStatus)
                .Include(wt => wt.TaskType)
                .Include(wt => wt.Reporter)
                .Include(wt => wt.AssignedUser)
                .Where(wt => wt.ProjectId == projectId && !wt.IsDeleted);

            // 3. Role-based filtering
            bool isManager = ManagerRoles.Contains(membership.ProjectRole, StringComparer.OrdinalIgnoreCase);

            if (!isManager)
            {
                // DEV, QA, etc. → chỉ thấy tasks được assign cho mình hoặc mình tạo
                query = query.Where(wt =>
                    wt.ReporterId == userId ||
                    wt.AssignedUserId == userId ||
                    wt.TaskAssignments.Any(ta => ta.UserId == userId)
                );
            }

            var tasks = await query
                .OrderByDescending(wt => wt.CreatedAt)
                .ToListAsync();

            // 4. Map → DTO with normalized statusName
            return tasks.Select(wt => MapToDto(wt)).ToList();
        }

        public async Task<WorkTaskResponseDto> CreateAsync(Guid reporterId, CreateWorkTaskDto request)
        {
            // 1. Kiểm tra user là member của project
            if (reporterId != Guid.Empty)
            {
                var isMember = await _context.ProjectMembers
                    .AnyAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == reporterId);
                if (!isMember)
                {
                    throw new UnauthorizedAccessException("Bạn không phải thành viên của dự án này. Không thể tạo công việc.");
                }
            }

            // 2. Resolve TaskStatus
            var taskStatus = await ResolveTaskStatusAsync(request.StatusName, request.ProjectId);

            // 3. Resolve TaskType
            var resolvedTaskTypeId = await ResolveTaskTypeIdAsync(request.TaskTypeId, request.TypeName, request.ProjectId);

            // 4. Fallback reporter
            if (reporterId == Guid.Empty)
            {
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser != null) reporterId = firstUser.Id;
            }

            // 5. Create entity
            var workTask = new TaskManagement.Domain.Entities.WorkTask
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                SprintId = request.SprintId,
                ParentTaskId = request.ParentTaskId,
                TaskTypeId = resolvedTaskTypeId,
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

            // Reload with navigation properties
            var created = await _context.WorkTasks
                .Include(wt => wt.TaskStatus)
                .Include(wt => wt.TaskType)
                .Include(wt => wt.Reporter)
                .Include(wt => wt.AssignedUser)
                .FirstAsync(wt => wt.Id == workTask.Id);

            return MapToDto(created);
        }

        public async Task UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequestDto request)
        {
            var taskToUpdate = await _context.WorkTasks
                .Include(wt => wt.TaskStatus)
                .Include(wt => wt.Sprint)
                .FirstOrDefaultAsync(wt => wt.Id == taskId && !wt.IsDeleted);

            if (taskToUpdate == null) throw new ArgumentException("Tác vụ không tồn tại.");

            // Sprint Lock
            if (taskToUpdate.Sprint != null && !taskToUpdate.Sprint.Status)
            {
                throw new InvalidOperationException("Không thể chỉnh sửa Task của một Sprint đã đóng.");
            }

            var oldStatus = taskToUpdate.TaskStatus;
            var newStatus = await _context.TaskStatuses.FirstOrDefaultAsync(ts => ts.Id == request.TaskStatusId);

            if (newStatus == null) throw new ArgumentException("Trạng thái chuyển đến không tồn tại.");
            if (oldStatus.Id == newStatus.Id) return;

            // State Machine Guardrails
            if (Math.Abs(oldStatus.Position - newStatus.Position) > 1)
            {
                throw new InvalidOperationException("Không được phép nhảy cóc trạng thái.");
            }

            bool isMovingToActiveOrDone = newStatus.Name.Contains("In Progress", StringComparison.OrdinalIgnoreCase) ||
                                          newStatus.Name.Contains("Done", StringComparison.OrdinalIgnoreCase) ||
                                          newStatus.Name.Contains("Đang làm", StringComparison.OrdinalIgnoreCase) ||
                                          newStatus.Name.Contains("Hoàn thành", StringComparison.OrdinalIgnoreCase);

            // Task Dependencies
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
                        throw new InvalidOperationException($"Tác vụ phụ thuộc '{blocker.Title}' chưa hoàn thành.");
                    }
                }
            }

            // Parent-Subtask Constraint
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
                    throw new InvalidOperationException("Không thể hoàn thành tác vụ cha khi vẫn còn subtask chưa hoàn thành.");
                }
            }

            taskToUpdate.TaskStatusId = newStatus.Id;
            taskToUpdate.UpdatedAt = DateTime.UtcNow;

            if (request.RowVersion != null && request.RowVersion.Length > 0)
            {
                _context.Entry(taskToUpdate).Property(nameof(taskToUpdate.RowVersion)).OriginalValue = request.RowVersion;
            }

            await _context.SaveChangesAsync();
        }

        // ==================== PRIVATE HELPERS ====================

        /// <summary>
        /// Normalize DB status name → standard frontend group name.
        /// Frontend expects: "TO DO", "IN PROGRESS", "DONE"
        /// </summary>
        private static string NormalizeStatusName(string? dbStatusName)
        {
            if (string.IsNullOrEmpty(dbStatusName)) return "TO DO";

            var upper = dbStatusName.ToUpper().Replace(" ", "");

            if (upper.Contains("DONE") || upper.Contains("HOANTHANH") || upper.Contains("COMPLETE"))
                return "DONE";
            if (upper.Contains("INPROGRESS") || upper.Contains("DANGLAM") || upper.Contains("ACTIVE"))
                return "IN PROGRESS";
            if (upper.Contains("TODO") || upper.Contains("CANLAM") || upper.Contains("BACKLOG"))
                return "TO DO";

            // For seed data like "Status 1", "Status 2", etc. → default to "TO DO"
            return "TO DO";
        }

        private WorkTaskResponseDto MapToDto(TaskManagement.Domain.Entities.WorkTask wt)
        {
            return new WorkTaskResponseDto
            {
                Id = wt.Id,
                Title = wt.Title,
                Description = wt.Description,
                ProjectId = wt.ProjectId,
                SprintId = wt.SprintId,
                TaskTypeId = wt.TaskTypeId,
                TypeName = wt.TaskType?.Name ?? "",
                TaskStatusId = wt.TaskStatusId,
                StatusName = NormalizeStatusName(wt.TaskStatus?.Name),
                ReporterId = wt.ReporterId,
                ReporterName = wt.Reporter?.FullName ?? "",
                AssignedUserId = wt.AssignedUserId,
                AssigneeName = wt.AssignedUser?.FullName,
                Priority = wt.Priority,
                StoryPoints = wt.StoryPoints,
                DueDate = wt.DueDate,
                PlannedStartDate = wt.PlannedStartDate,
                PlannedEndDate = wt.PlannedEndDate,
                CreatedAt = wt.CreatedAt,
                UpdatedAt = wt.UpdatedAt,
                RowVersion = wt.RowVersion
            };
        }

        private async Task<TaskManagement.Domain.Entities.TaskStatus> ResolveTaskStatusAsync(string? statusName, Guid projectId)
        {
            TaskManagement.Domain.Entities.TaskStatus? taskStatus = null;

            // Try exact match
            if (!string.IsNullOrEmpty(statusName))
            {
                taskStatus = await _context.TaskStatuses
                    .FirstOrDefaultAsync(ts => ts.Name.ToUpper().Replace(" ", "") == statusName.ToUpper().Replace(" ", ""));
            }
            // Try known default names
            if (taskStatus == null)
            {
                taskStatus = await _context.TaskStatuses
                    .FirstOrDefaultAsync(ts => ts.Name.Contains("To Do") || ts.Name.Contains("Cần làm"));
            }
            // Fallback: first status in project
            if (taskStatus == null)
            {
                taskStatus = await _context.TaskStatuses
                    .FirstOrDefaultAsync(ts => ts.ProjectId == projectId);
            }
            // Ultimate fallback: any status
            if (taskStatus == null)
            {
                taskStatus = await _context.TaskStatuses.FirstOrDefaultAsync();
            }

            return taskStatus ?? throw new InvalidOperationException("Không tìm thấy trạng thái nào. Vui lòng tạo trạng thái trước.");
        }

        private async Task<Guid> ResolveTaskTypeIdAsync(Guid? taskTypeId, string? typeName, Guid projectId)
        {
            if (taskTypeId.HasValue && taskTypeId.Value != Guid.Empty)
            {
                return taskTypeId.Value;
            }

            TaskManagement.Domain.Entities.TaskType? taskType = null;

            if (!string.IsNullOrEmpty(typeName))
            {
                taskType = await _context.TaskTypes
                    .FirstOrDefaultAsync(tt => tt.Name.ToUpper() == typeName.ToUpper());
            }
            if (taskType == null)
            {
                taskType = await _context.TaskTypes
                    .FirstOrDefaultAsync(tt => tt.ProjectId == projectId);
            }
            if (taskType == null)
            {
                taskType = await _context.TaskTypes.FirstOrDefaultAsync();
            }

            return taskType?.Id ?? throw new InvalidOperationException("Không tìm thấy loại tác vụ nào.");
        }
    }
}
