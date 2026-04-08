using System;

namespace TaskManagement.Application.DTOs.WorkTask
{
    public class UpdateWorkTaskDto
    {
<<<<<<< HEAD
        public Guid? SprintId { get; set; }
        public Guid TaskStatusId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; }
        public double StoryPoints { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? StatusName { get; set; }
        public string? TypeName { get; set; }
        public byte[] RowVersion { get; set; } = null!;
=======
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
>>>>>>> 48c51e8ea7724f864eee16b488d26cc33f3752ec
    }
}
