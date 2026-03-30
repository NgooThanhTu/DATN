using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Sprint
{
    public class CreateSprintDto
    {
        [Required(ErrorMessage = "Tên Sprint là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên Sprint không quá 200 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        public DateTime EndDate { get; set; }
    }
}
