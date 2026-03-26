using TaskManagement.Domain.Entities;
using System.Security.Claims;

namespace TaskManagement.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, IList<string> systemRoles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
