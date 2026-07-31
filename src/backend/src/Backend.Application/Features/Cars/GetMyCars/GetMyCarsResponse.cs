namespace Backend.Application.Features.Cars.GetMyCars;

public sealed record GetMyCarsResponse(
    Guid Id,
    string LicensePlate,
    string Brand,
    string Model,
    int Year,
    string VinNumber,
    string Status
);
