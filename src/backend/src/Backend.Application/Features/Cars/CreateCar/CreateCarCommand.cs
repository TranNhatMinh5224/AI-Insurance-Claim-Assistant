using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Cars.CreateCar;

public sealed record CreateCarCommand(
    string LicensePlate,
    string Brand,
    string Model,
    int ManufacturingYear
) : IRequest<Result<CreateCarResponse>>;
