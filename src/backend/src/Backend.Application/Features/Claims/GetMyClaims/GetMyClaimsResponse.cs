namespace Backend.Application.Features.Claims.GetMyClaims;

public sealed record GetMyClaimsResponse(
    Guid Id,
    Guid PolicyId,
    string IncidentDescription,
    string Status,
    DateTime CreatedAt
);
