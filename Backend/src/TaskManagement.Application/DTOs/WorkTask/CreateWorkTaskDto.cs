using System;
<<<<<<< HEAD
=======
using System.ComponentModel.DataAnnotations;
>>>>>>> 48c51e8ea7724f864eee16b488d26cc33f3752ec

namespace TaskManagement.Application.DTOs.WorkTask
{
    public class CreateWorkTaskDto
    {
<<<<<<< HEAD
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
        public Guid? AssignedUserId { get; set; }
        public string? StatusName { get; set; }
        public string? TypeName { get; set; }
=======
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Guid ProjectId { get; set; }

        public Guid? SprintId { get; set; }
        
        public Guid? ParentTaskId { get; set; }
        
        // Support both Guid-based and Name-based lookup
        public Guid? TaskTypeId { get; set; }
        public string? TypeName { get; set; }

        public string? StatusName { get; set; }

        public int Priority { get; set; } = 3;
        public double StoryPoints { get; set; }

        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? DueDate { get; set; }

        public Guid? AssignedUserId { get; set; }
        public double TotalEstimatedHours { get; set; }
>>>>>>> 48c51e8ea7724f864eee16b488d26cc33f3752ec
    }
}
