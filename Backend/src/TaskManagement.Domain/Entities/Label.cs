using System;
using System.Collections.Generic;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Nhãn phân loại cho Task. Ví dụ: "Bug", "Feature", "Urgent", "Backend"
    /// Mỗi Label thuộc về 1 Project (hoặc Workspace-level nếu ProjectId null).
    /// </summary>
    public class Label
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ColorCode { get; set; } = "#3b82f6";
        public string? Description { get; set; }

        public Guid? ProjectId { get; set; }
        public Project? Project { get; set; }

        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<IssueLabel> IssueLabels { get; set; } = new List<IssueLabel>();
    }
}
