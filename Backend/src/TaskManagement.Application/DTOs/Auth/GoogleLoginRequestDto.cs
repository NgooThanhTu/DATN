using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Auth
{
    public class GoogleLoginRequestDto
    {
        [Required]
        public string Credential { get; set; } = string.Empty;
    }
}
