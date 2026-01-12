using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        int? ValidateToken(string token);
        DateTime GetTokenExpiration();
        DateTime GetRefreshTokenExpiration();
    }
}
