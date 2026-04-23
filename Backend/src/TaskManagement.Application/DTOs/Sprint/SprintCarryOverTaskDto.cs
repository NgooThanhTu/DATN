namespace TaskManagement.Application.DTOs.Sprint
{
    public class SprintCarryOverTaskDto
    {
        public Guid Id { get; set; }
        public string? SequenceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        public Guid? CurrentSprintId { get; set; }
        public string? CurrentSprintName { get; set; }
        public string CurrentLocation { get; set; } = "Backlog";
        public DateTime UpdatedAt { get; set; }
    }
}
