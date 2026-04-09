using System;

namespace TaskManagement.Domain.Entities
{
    public class SystemAuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? IPAddress { get; set; }
        public string? Details { get; set; } // JSON or text details
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
