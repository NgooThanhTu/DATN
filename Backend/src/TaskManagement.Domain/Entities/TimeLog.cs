using System;

namespace TaskManagement.Domain.Entities
{
    public class TimeLog
    {
        public Guid Id { get; set; }
        public Guid WorkTaskId { get; set; }
        public WorkTask WorkTask { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public double Hours { get; set; }
        public string WorkType { get; set; } = string.Empty; // NEW_CODE, FIX_BUG, REVIEW...
        public string? Note { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}
