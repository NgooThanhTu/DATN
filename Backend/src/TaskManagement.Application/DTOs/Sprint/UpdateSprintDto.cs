using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Sprint
{
    public class UpdateSprintDto
    {
        [Required(ErrorMessage = "Tên Sprint là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên Sprint không được vượt quá 255 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        public DateTime EndDate { get; set; }
    }
}
