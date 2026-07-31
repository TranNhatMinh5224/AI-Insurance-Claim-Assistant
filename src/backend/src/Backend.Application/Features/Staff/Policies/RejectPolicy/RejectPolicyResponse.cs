namespace Backend.Application.Features.Staff.Policies.RejectPolicy;

public sealed record RejectPolicyResponse(
    Guid PolicyId,
    string Status
);
