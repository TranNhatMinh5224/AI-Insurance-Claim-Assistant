using System.Security.Claims;
using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
