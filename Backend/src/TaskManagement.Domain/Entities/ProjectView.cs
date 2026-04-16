using System;

namespace TaskManagement.Domain.Entities
{
    public class ProjectView
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        /// <summary>
        /// JSON Document stored filters. e.g. { "Status": ["Todo", "InProgress"], "Priority": "Urgent" }
        /// </summary>
        public string QueryMetadata { get; set; } = "{}";

        public bool IsFavorite { get; set; } = false;
        
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
