using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Auth
{
    public class AcceptInviteRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên của bạn")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu muốn sử dụng")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; } = string.Empty;
    }
}
