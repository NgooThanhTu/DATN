using System;
using System.Threading.Tasks;

namespace TaskManagement.Application.Interfaces
{
    public interface IGamificationService
    {
        Task ApplyStatusChangeRewardsAsync(Guid workTaskId, Guid actorUserId, string? oldStatusName, string? newStatusName);
        Task ApplyAssignmentProgressRewardsAsync(Guid workTaskId, Guid assigneeUserId, Guid actorUserId, double oldProgressPercent, double newProgressPercent);
    }
}
