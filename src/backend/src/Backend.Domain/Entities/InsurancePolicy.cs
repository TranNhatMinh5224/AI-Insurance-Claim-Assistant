using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public sealed class InsurancePolicy
{
    private InsurancePolicy() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CarId { get; private set; }
    public Guid PackageId { get; private set; }
    public Guid PolicyTermId { get; private set; }
    public PolicyStatus Status { get; private set; }

    /// <summary>
    /// Phí bảo hiểm đã "đóng băng" tại thời điểm ký hợp đồng (VND).
    /// Không thay đổi dù gói bảo hiểm sau này có đổi giá.
    /// </summary>
    public decimal PremiumAmount { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? EPolicyPdfUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static InsurancePolicy Create(Guid userId, Guid carId, Guid packageId, Guid policyTermId, decimal premiumAmount)
    {
        return new InsurancePolicy
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CarId = carId,
            PackageId = packageId,
            PolicyTermId = policyTermId,
            PremiumAmount = premiumAmount,
            Status = PolicyStatus.PendingApproval,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void ActivatePolicy(string ePolicyPdfUrl, int durationMonths = 12)
    {
        Status = PolicyStatus.Active;
        StartDate = DateTime.UtcNow;
        EndDate = StartDate.AddMonths(durationMonths);
        EPolicyPdfUrl = ePolicyPdfUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelPolicy()
    {
        Status = PolicyStatus.Canceled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ExpirePolicy()
    {
        Status = PolicyStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RejectPolicy()
    {
        Status = PolicyStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }
}
