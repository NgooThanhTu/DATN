using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.WorkTask;

namespace TaskManagement.Application.Interfaces
{
    public interface IWorkTaskService
    {
        Task UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequestDto request);
        Task<IEnumerable<WorkTaskResponseDto>> GetTasksByProjectIdAsync(Guid projectId);
        Task<IEnumerable<WorkTaskResponseDto>> GetMyTasksAsync(Guid userId);
    }
}
