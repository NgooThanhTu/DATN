namespace TaskManagement.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DeviceId { get; set; } = "WEB-CLIENT-" + Guid.NewGuid().ToString().Substring(0, 8);
    }
}
