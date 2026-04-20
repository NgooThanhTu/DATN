using System;

namespace TaskManagement.Domain.Entities
{
    public class TaskAssignment
    {
        public Guid WorkTaskId { get; set; }
        public WorkTask WorkTask { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public bool Status { get; set; } = false;
        public double ProgressPercent { get; set; } = 0;
        public double ContributionWeight { get; set; } = 1;
        public Guid? BlockedByUserId { get; set; }
        public User? BlockedByUser { get; set; }
        public string? BlockReason { get; set; }
        public DateTime? ProgressUpdatedAt { get; set; }
        public int Priority { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public string? Description { get; set; }
        public double EstimatedHours { get; set; }
        public double TotalActualHours { get; set; }
    }
}
