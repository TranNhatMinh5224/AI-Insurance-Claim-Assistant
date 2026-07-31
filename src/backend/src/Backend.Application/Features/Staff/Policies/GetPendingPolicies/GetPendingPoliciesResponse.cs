namespace Backend.Application.Features.Staff.Policies.GetPendingPolicies;

public sealed record GetPendingPoliciesResponse(
    Guid Id,
    Guid UserId,
    Guid CarId,
    Guid PackageId,
    decimal PremiumAmount,
    DateTime CreatedAt
);
