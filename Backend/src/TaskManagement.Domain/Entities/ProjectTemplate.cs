using System;
using System.Collections.Generic;

namespace TaskManagement.Domain.Entities
{
    public class ProjectTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., Basic IT service management
        public string TemplateCode { get; set; } = string.Empty; // e.g., IT_SERVICE
        public string? Description { get; set; }
        
        // Cấu hình mặc định
        public string? DefaultNavigationConfig { get; set; } // JSON format
        
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
