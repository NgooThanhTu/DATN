using System;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.WorkTask;

namespace TaskManagement.Application.Interfaces
{
    public interface IWorkTaskService
    {
        Task<WorkTaskResponseDto> CreateAsync(Guid reporterId, CreateWorkTaskDto request);
        Task UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequestDto request);
    }
}
