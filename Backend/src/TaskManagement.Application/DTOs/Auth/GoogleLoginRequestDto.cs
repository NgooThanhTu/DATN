using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagement.Application.DTOs.Auth
{
    public class GoogleLoginRequestDto
    {
        [Required]
        [JsonPropertyName("credential")]
        public string Credential { get; set; } = string.Empty;
    }
}