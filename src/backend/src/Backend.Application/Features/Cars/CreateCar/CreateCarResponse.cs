namespace Backend.Application.Features.Cars.CreateCar;

public sealed record CreateCarResponse(
    Guid CarId,
    string LicensePlate,
    string Brand,
    string Model,
    int ManufacturingYear
);
