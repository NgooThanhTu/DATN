using System;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// M:N mapping giữa WorkTask và Label.
    /// </summary>
    public class IssueLabel
    {
        public Guid WorkTaskId { get; set; }
        public WorkTask WorkTask { get; set; } = null!;

        public Guid LabelId { get; set; }
        public Label Label { get; set; } = null!;

        public DateTime AssignedAt { get; set; }
    }
}
