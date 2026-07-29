namespace Backend.Application.Features.Auth.Login;

public sealed record LoginResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
