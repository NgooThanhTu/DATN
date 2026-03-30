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
        public Guid TaskStatusId { get; set; }
        public Guid ReporterId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
