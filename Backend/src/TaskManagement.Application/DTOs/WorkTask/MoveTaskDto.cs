using System;

namespace TaskManagement.Application.DTOs.WorkTask
{
    public class MoveTaskDto
    {
        public Guid TaskStatusId { get; set; }
        public string? StatusName { get; set; }
        public byte[] RowVersion { get; set; } = null!;
    }
}
