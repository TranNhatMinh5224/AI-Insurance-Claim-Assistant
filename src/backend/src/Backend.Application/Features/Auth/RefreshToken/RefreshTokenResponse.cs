namespace Backend.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
