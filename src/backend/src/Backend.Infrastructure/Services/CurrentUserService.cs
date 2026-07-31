using Backend.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Backend.Infrastructure.Services;

internal sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid GetUserIdOrThrow()
        => UserId ?? throw new UnauthorizedAccessException("Không xác định được người dùng từ token.");

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated is true;
}
