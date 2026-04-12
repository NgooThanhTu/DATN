using System;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Mapping between User and Workspace. Mỗi user có 1 WorkspaceRole trong workspace đó.
    /// WorkspaceRole: "OWNER", "ADMIN", "MEMBER", "GUEST"
    /// </summary>
    public class WorkspaceMember
    {
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        /// <summary>
        /// Role trong workspace: OWNER, ADMIN, MEMBER, GUEST
        /// </summary>
        public string WorkspaceRole { get; set; } = "MEMBER";

        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
