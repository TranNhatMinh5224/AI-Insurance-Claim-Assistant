namespace Backend.Domain.Entities;

public sealed class Car
{
    private Car() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string LicensePlate { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int ManufacturingYear { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Car Create(Guid userId, string licensePlate, string brand, string model, int year)
    {
        return new Car
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LicensePlate = licensePlate.ToUpperInvariant().Trim(),
            Brand = brand.Trim(),
            Model = model.Trim(),
            ManufacturingYear = year,
            CreatedAt = DateTime.UtcNow
        };
    }
}
