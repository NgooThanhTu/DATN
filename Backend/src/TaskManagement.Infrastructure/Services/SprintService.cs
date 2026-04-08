using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs.Sprint;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Services
{
    public class SprintService : ISprintService
    {
        private readonly ApplicationDbContext _context;

        public SprintService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SprintResponseDto>> GetByProjectAsync(Guid projectId)
        {
            return await _context.Sprints
                .Where(s => s.ProjectId == projectId)
                .Select(s => new SprintResponseDto
                {
                    Id = s.Id,
                    ProjectId = s.ProjectId,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Status = s.Status,
                    TaskCount = s.WorkTasks.Count(),
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<SprintResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Sprints
                .Where(s => s.Id == id)
                .Select(s => new SprintResponseDto
                {
                    Id = s.Id,
                    ProjectId = s.ProjectId,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Status = s.Status,
                    TaskCount = s.WorkTasks.Count(),
                    CreatedAt = s.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<SprintResponseDto> CreateAsync(Guid projectId, CreateSprintDto dto)
        {
            // Validate project tồn tại
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                throw new ArgumentException("Dự án không tồn tại.");

            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");

            var sprint = new Sprint
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = false, // Mới tạo chưa Active, phải gọi Start
                CreatedAt = DateTime.UtcNow
            };

            _context.Sprints.Add(sprint);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(sprint.Id))!;
        }

        public async Task<SprintResponseDto> UpdateAsync(Guid projectId, Guid sprintId, UpdateSprintDto dto)
        {
            var sprint = await _context.Sprints
                .FirstOrDefaultAsync(s => s.Id == sprintId && s.ProjectId == projectId);

            if (sprint == null)
                throw new ArgumentException("Sprint không tồn tại trong dự án này.");

            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");

            sprint.Name = dto.Name;
            sprint.StartDate = dto.StartDate;
            sprint.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();

            return (await GetByIdAsync(sprint.Id))!;
        }

        /// <summary>
        /// 5.3 Sprint Overlap Guard: Chặn 2 Sprint cùng Active trong 1 Project
        /// </summary>
        public async Task<SprintResponseDto> StartAsync(Guid projectId, Guid sprintId)
        {
            var sprint = await _context.Sprints
                .FirstOrDefaultAsync(s => s.Id == sprintId && s.ProjectId == projectId);

            if (sprint == null)
                throw new ArgumentException("Sprint không tồn tại trong dự án này.");

            if (sprint.Status)
                throw new InvalidOperationException("Sprint này đã đang chạy.");

            // === SPRINT OVERLAP GUARD ===
            bool hasActiveSprint = await _context.Sprints
                .AnyAsync(s => s.ProjectId == projectId && s.Status == true);

            if (hasActiveSprint)
                throw new InvalidOperationException("Dự án đang có Sprint đang chạy! Phải đóng Sprint hiện tại trước khi bắt đầu Sprint mới.");

            sprint.Status = true;
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(sprint.Id))!;
        }

        /// <summary>
        /// 5.5 Close Sprint + Roll-over Tasks (bọc trong Transaction)
        /// </summary>
        public async Task CloseAsync(Guid sprintId, CloseSprintDto dto)
        {
            var sprint = await _context.Sprints
                .FirstOrDefaultAsync(s => s.Id == sprintId);

            if (sprint == null)
                throw new ArgumentException("Sprint không tồn tại.");

            if (!sprint.Status)
                throw new InvalidOperationException("Sprint này đã đóng.");

            // Validate target sprint nếu có
            if (dto.TargetSprintId.HasValue)
            {
                var targetSprint = await _context.Sprints
                    .FirstOrDefaultAsync(s => s.Id == dto.TargetSprintId.Value && s.ProjectId == sprint.ProjectId);
                if (targetSprint == null)
                    throw new ArgumentException("Sprint đích không tồn tại trong cùng dự án.");
            }

            // === BỌC TRONG TRANSACTION (theo CONTEXT.md) ===
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tìm tất cả TaskStatus có Name chứa "Done" hoặc "Hoàn thành" trong project
                var doneStatusIds = await _context.TaskStatuses
                    .Where(ts => ts.ProjectId == sprint.ProjectId &&
                                 (ts.Name.Contains("Done") || ts.Name.Contains("Hoàn thành")))
                    .Select(ts => ts.Id)
                    .ToListAsync();

                // Chuyển các task chưa Done sang sprint mới hoặc backlog (null)
                var unfinishedTasks = await _context.WorkTasks
                    .Where(wt => wt.SprintId == sprintId && !doneStatusIds.Contains(wt.TaskStatusId))
                    .ToListAsync();

                foreach (var task in unfinishedTasks)
                {
                    task.SprintId = dto.TargetSprintId; // null = backlog
                    task.UpdatedAt = DateTime.UtcNow;
                }

                // Đóng sprint
                sprint.Status = false;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
