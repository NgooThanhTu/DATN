namespace TaskManagement.Application.DTOs.Sprint
{
    public class BulkMoveCarryOverTasksDto
    {
        public List<Guid> TaskIds { get; set; } = new();
        public Guid? TargetSprintId { get; set; }
    }
}
