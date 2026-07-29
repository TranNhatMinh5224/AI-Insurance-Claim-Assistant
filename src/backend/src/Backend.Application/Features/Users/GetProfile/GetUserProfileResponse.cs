namespace Backend.Application.Features.Users.GetProfile;

public sealed record GetUserProfileResponse(
    Guid UserId,
    string FullName,
    string Email,
    string? PhoneNumber,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
