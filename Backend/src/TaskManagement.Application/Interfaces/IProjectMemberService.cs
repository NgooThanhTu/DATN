using System;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Project;

namespace TaskManagement.Application.Interfaces
{
    public interface IProjectMemberService
    {
        Task InviteMemberAsync(Guid projectId, ProjectMemberRequestDto request);
        Task RemoveMemberAsync(Guid projectId, Guid userId);
        Task UpdateMemberRoleAsync(Guid projectId, Guid userId, string newRole);
        Task<System.Collections.Generic.IEnumerable<ProjectMemberResponseDto>> GetProjectMembersAsync(Guid projectId);
    }
}
