using System;

namespace TaskManagement.Domain.Entities
{
    public class ProjectMember
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string ProjectRole { get; set; } = string.Empty; // PROJECT_MANAGER, DEV, TESTER...
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool Status { get; set; } = false;
    }
}
