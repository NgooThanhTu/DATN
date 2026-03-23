using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthResponseDto response, string refreshToken)> LoginAsync(LoginRequestDto request);
        Task RegisterAsync(RegisterRequestDto request);
        Task<(string newAccessToken, string newRefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken);
        Task RevokeTokenAsync(Guid userId);
    }
}
