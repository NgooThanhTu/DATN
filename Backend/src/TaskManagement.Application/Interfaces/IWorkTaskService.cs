using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.WorkTask;

namespace TaskManagement.Application.Interfaces
{
    public interface IWorkTaskService
    {
<<<<<<< HEAD
        Task<IEnumerable<WorkTaskDto>> GetTasksByProjectAsync(Guid projectId);
        Task<WorkTaskDto?> GetTaskByIdAsync(Guid taskId);
        Task<WorkTaskDto> CreateTaskAsync(Guid userId, CreateWorkTaskDto dto);
        Task<WorkTaskDto?> UpdateTaskAsync(Guid userId, Guid taskId, UpdateWorkTaskDto dto);
        Task<WorkTaskDto?> MoveTaskAsync(Guid userId, Guid taskId, MoveTaskDto dto);
        Task<bool> DeleteTaskAsync(Guid userId, Guid taskId);
=======
        /// <summary>
        /// Get tasks by project with role-based authorization.
        /// PM/PO/SM see all tasks. DEV/QA see only assigned/reported tasks.
        /// </summary>
        Task<List<WorkTaskResponseDto>> GetByProjectAsync(Guid projectId, Guid userId);
        
        Task<WorkTaskResponseDto> CreateAsync(Guid reporterId, CreateWorkTaskDto request);
        Task<WorkTaskResponseDto> UpdateAsync(Guid taskId, Guid userId, UpdateWorkTaskDto dto);
        Task UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequestDto request);
        Task<IEnumerable<WorkTaskResponseDto>> GetTasksByProjectIdAsync(Guid projectId);
        Task<IEnumerable<WorkTaskResponseDto>> GetMyTasksAsync(Guid userId);
>>>>>>> 48c51e8ea7724f864eee16b488d26cc33f3752ec
    }
}
