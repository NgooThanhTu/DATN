using System;

namespace TaskManagement.Application.DTOs.WorkTask
{
    public class WorkTaskDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? SprintId { get; set; }
        public Guid? ParentTaskId { get; set; }
        public Guid TaskTypeId { get; set; }
        public Guid TaskStatusId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; }
        public double StoryPoints { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public Guid ReporterId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public string StatusName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string? AssigneeName { get; set; }
    }
}
