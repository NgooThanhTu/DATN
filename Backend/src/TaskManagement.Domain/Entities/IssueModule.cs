using System;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// M:N mapping giữa WorkTask và Module.
    /// </summary>
    public class IssueModule
    {
        public Guid WorkTaskId { get; set; }
        public WorkTask WorkTask { get; set; } = null!;

        public Guid ModuleId { get; set; }
        public Module Module { get; set; } = null!;

        public DateTime AssignedAt { get; set; }
    }
}
