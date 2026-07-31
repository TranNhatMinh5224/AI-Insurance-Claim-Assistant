using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public sealed class ClaimRequest
{
    private ClaimRequest() { }

    public Guid Id { get; private set; }
    public Guid InsurancePolicyId { get; private set; }
    public string IncidentDescription { get; private set; } = string.Empty;
    public ClaimStatus Status { get; private set; }
    public Guid? AssignedStaffId { get; private set; }
    public string? StaffNote { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static ClaimRequest Create(Guid policyId, string description)
    {
        return new ClaimRequest
        {
            Id = Guid.NewGuid(),
            InsurancePolicyId = policyId,
            IncidentDescription = description,
            Status = ClaimStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(ClaimStatus newStatus, Guid? staffId = null, string? note = null)
    {
        Status = newStatus;
        if (staffId.HasValue) AssignedStaffId = staffId.Value;
        if (note != null) StaffNote = note;
        UpdatedAt = DateTime.UtcNow;
    }
}
