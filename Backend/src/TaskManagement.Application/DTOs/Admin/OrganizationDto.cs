using System;

namespace TaskManagement.Application.DTOs.Admin
{
    public class OrganizationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? CompanySize { get; set; }
    }
}
