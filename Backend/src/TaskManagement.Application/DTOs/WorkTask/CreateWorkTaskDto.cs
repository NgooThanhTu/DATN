using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.WorkTask
{
    public class CreateWorkTaskDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Dự án không được để trống.")]
        public Guid ProjectId { get; set; }

        public Guid? SprintId { get; set; }
        
        public Guid? ParentTaskId { get; set; }
        
        [Required(ErrorMessage = "Loại tác vụ không được để trống.")]
        public Guid TaskTypeId { get; set; }

        public int Priority { get; set; }
        public double StoryPoints { get; set; }

        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? DueDate { get; set; }

        public Guid? AssignedUserId { get; set; }
        public double TotalEstimatedHours { get; set; }
    }
}
