namespace Backend.Application.Features.Staff.Claims.GetAllClaims;

public sealed record GetAllClaimsResponse(
    Guid Id,
    Guid InsurancePolicyId,
    string IncidentDescription,
    string Status,
    Guid? AssignedStaffId,
    DateTime CreatedAt
);
