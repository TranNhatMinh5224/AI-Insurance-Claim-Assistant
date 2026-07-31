namespace Backend.Application.Features.Policies.CreatePolicy;

public sealed record CreatePolicyResponse(
    Guid PolicyId,
    Guid CarId,
    Guid PackageId,
    decimal PremiumAmount,
    string Status
);
