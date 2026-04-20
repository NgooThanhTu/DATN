using System;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Trang tài liệu Wiki tích hợp vào Project.
    /// Block-based editor dạng Notion thu nhỏ.
    /// </summary>
    public class Page
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        /// <summary>Nội dung trang dạng JSON (block editor format / Tiptap)</summary>
        public string? Content { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public Guid? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }

        /// <summary>Sắp xếp thứ tự hiển thị</summary>
        public int SortOrder { get; set; }

        /// <summary>Trang có bị khóa chỉnh sửa không</summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>Trang có bị archive không</summary>
        public bool IsArchived { get; set; } = false;

        public bool IsPrivate { get; set; } = false;
        public bool IsStarred { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
