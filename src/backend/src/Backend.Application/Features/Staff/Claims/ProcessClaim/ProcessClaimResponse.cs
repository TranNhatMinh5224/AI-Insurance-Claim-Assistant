namespace Backend.Application.Features.Staff.Claims.ProcessClaim;

public sealed record ProcessClaimResponse(
    Guid ClaimId,
    string Status,
    Guid StaffId,
    string? StaffNote
);
