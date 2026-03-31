using System;

namespace TaskManagement.Application.DTOs.WorkTask
{
    public class UpdateWorkTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public int? StoryPoints { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? SprintId { get; set; }
        public Guid TaskTypeId { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
