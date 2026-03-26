using System;
using System.Collections.Generic;

namespace TaskManagement.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ManagerId { get; set; }
        public DateTime CreatedAt { get; set; }

        // New properties
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public User? Manager { get; set; }
        public ICollection<DepartmentMember> DepartmentMembers { get; set; } = new List<DepartmentMember>();
    }
}
