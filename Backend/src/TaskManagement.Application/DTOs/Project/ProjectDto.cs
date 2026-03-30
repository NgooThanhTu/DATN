using System;

namespace TaskManagement.Application.DTOs.Project
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Hỗ trợ hiển thị frontend
        public string Type { get; set; } = "Dự án phần mềm";
        public string Icon { get; set; } = "fa-brands fa-jira";
    }
}
