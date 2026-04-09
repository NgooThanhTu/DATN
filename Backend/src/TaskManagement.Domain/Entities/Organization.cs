using System;

namespace TaskManagement.Domain.Entities
{
    public class Organization
    {
        public string Id { get; set; } = string.Empty; // Using string to support 'org_8f3kd92jd0s' like UI
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? CompanySize { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
