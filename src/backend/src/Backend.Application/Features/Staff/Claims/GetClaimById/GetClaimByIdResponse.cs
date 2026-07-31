namespace Backend.Application.Features.Staff.Claims.GetClaimById;

public sealed record GetClaimByIdResponse(
    Guid Id,
    Guid InsurancePolicyId,
    string IncidentDescription,
    string Status,
    Guid? AssignedStaffId,
    string? StaffNote,
    DateTime CreatedAt,
    List<EvidenceDto> Evidences
);

public sealed record EvidenceDto(
    Guid Id,
    string EvidenceType,
    string ImageUrl,
    DateTime CreatedAt
);
