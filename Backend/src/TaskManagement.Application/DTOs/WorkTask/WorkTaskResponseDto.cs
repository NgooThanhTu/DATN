using System;

namespace TaskManagement.Application.DTOs.WorkTask
{
    public class WorkTaskResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? SprintId { get; set; }
        public Guid TaskTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public Guid TaskStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid ReporterId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public Guid? AssignedUserId { get; set; }
        public string? AssigneeName { get; set; }
        public int Priority { get; set; }
        public double StoryPoints { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
