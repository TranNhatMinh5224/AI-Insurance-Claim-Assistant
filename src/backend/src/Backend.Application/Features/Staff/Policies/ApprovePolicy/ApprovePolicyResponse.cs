namespace Backend.Application.Features.Staff.Policies.ApprovePolicy;

public sealed record ApprovePolicyResponse(
    Guid PolicyId,
    string Status,
    string EPolicyPdfUrl,
    DateTime StartDate,
    DateTime EndDate
);
