using System;

namespace TaskManagement.Application.DTOs.Admin
{
    public class SystemAuditLogDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? IPAddress { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
