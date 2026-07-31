namespace Backend.Application.Features.Policies.GetMyPolicies;

public sealed record GetMyPoliciesResponse(
    Guid Id,
    Guid CarId,
    Guid PackageId,
    string Status,
    decimal PremiumAmount,
    DateTime StartDate,
    DateTime EndDate,
    string? EPolicyPdfUrl,
    DateTime CreatedAt
);
