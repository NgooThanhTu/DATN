using System;
using System.Threading.Tasks;

namespace TaskManagement.Application.Interfaces
{
    public interface IProjectMemberService
    {
        Task RemoveMemberAsync(Guid projectId, Guid userId, Guid adminId);
    }
}
