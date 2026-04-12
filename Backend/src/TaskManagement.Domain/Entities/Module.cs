using System;
using System.Collections.Generic;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Module (tương đương Epic/Feature trong Plane).
    /// Đánh dấu các cột mốc phát triển lớn, nhóm nhiều Issues.
    /// Không bị giới hạn thời gian như Cycle/Sprint.
    /// </summary>
    public class Module
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        /// <summary>Ngày bắt đầu dự kiến (tùy chọn)</summary>
        public DateTime? StartDate { get; set; }

        /// <summary>Ngày kết thúc dự kiến (tùy chọn)</summary>
        public DateTime? TargetDate { get; set; }

        /// <summary>Trạng thái: Backlog, Planned, InProgress, Paused, Completed, Cancelled</summary>
        public string Status { get; set; } = "Backlog";

        public Guid? LeadId { get; set; }
        public User? Lead { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public ICollection<IssueModule> IssueModules { get; set; } = new List<IssueModule>();
    }
}
