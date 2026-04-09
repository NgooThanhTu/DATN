using System;

namespace TaskManagement.Domain.Entities
{
    public class ProjectDepartmentRole
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public string RoleName { get; set; } = string.Empty; // VD: Service Desk Team, Customer
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
