using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.WorkTask;

namespace TaskManagement.Application.Interfaces
{
    public interface IWorkTaskService
    {
        /// <summary>
        /// Get tasks by project with role-based authorization.
        /// PM/PO/SM see all tasks. DEV/QA see only assigned/reported tasks.
        /// </summary>
        Task<List<WorkTaskResponseDto>> GetByProjectAsync(Guid projectId, Guid userId);
        
        Task<WorkTaskResponseDto> CreateAsync(Guid reporterId, CreateWorkTaskDto request);
        Task UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequestDto request);
    }
}
