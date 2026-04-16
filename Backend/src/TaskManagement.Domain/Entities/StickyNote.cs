using System;

namespace TaskManagement.Domain.Entities
{
    public class StickyNote
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public string Color { get; set; } = "#FEF08A"; // Default yellow
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
