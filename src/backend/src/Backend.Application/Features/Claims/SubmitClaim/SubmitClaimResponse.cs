namespace Backend.Application.Features.Claims.SubmitClaim;

public sealed record SubmitClaimResponse(
    Guid ClaimId,
    string Status,
    int EvidenceCount,
    DateTime CreatedAt
);
