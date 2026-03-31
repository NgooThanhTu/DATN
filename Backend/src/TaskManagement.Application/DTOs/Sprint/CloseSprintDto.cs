namespace TaskManagement.Application.DTOs.Sprint
{
    /// <summary>
    /// DTO dùng khi đóng Sprint. targetSprintId = null nghĩa là đẩy task tồn đọng về Backlog.
    /// </summary>
    public class CloseSprintDto
    {
        public Guid? TargetSprintId { get; set; }
    }
}
