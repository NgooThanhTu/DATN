using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Admin
{
    public class AddUserRequestDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò là bắt buộc.")]
        public string Role { get; set; } = "Developer"; // Developer, Admin, etc.

        // Có thể gán vào dự án ngay lập tức
        public Guid? ProjectId { get; set; }
    }
}
