namespace Backend.Application.Features.Policies.CancelPolicy;

public sealed record CancelPolicyResponse(
    Guid PolicyId,
    string Status
);
