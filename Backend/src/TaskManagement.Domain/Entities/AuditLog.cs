using System;

namespace TaskManagement.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid WorkTaskId { get; set; }
        public WorkTask WorkTask { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string FieldChanged { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
