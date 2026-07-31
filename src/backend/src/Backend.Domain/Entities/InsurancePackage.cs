namespace Backend.Domain.Entities;

public sealed class InsurancePackage
{
    private InsurancePackage() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    /// <summary>Phí bảo hiểm cơ bản tính theo năm (VND).</summary>
    public decimal BasePrice { get; private set; }

    /// <summary>Mô tả tóm tắt các quyền lợi bảo hiểm (tổng mức bồi thường tối đa, quyền lợi đặc biệt...).</summary>
    public string CoverageDescription { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static InsurancePackage Create(string name, string description, decimal basePrice, string coverageDescription)
    {
        return new InsurancePackage
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            BasePrice = basePrice,
            CoverageDescription = coverageDescription,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
