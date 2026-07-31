using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using MediatR;

namespace Backend.Application.Features.Cars.CreateCar;

internal sealed class CreateCarCommandHandler
    : IRequestHandler<CreateCarCommand, Result<CreateCarResponse>>
{
    private readonly ICarRepository _carRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateCarCommandHandler(
        ICarRepository carRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _carRepo = carRepo;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<CreateCarResponse>> Handle(CreateCarCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<CreateCarResponse>.Failure(
                Error.Unauthorized("Auth.Unauthenticated", "Không xác định được người dùng."));

        var licensePlateNormalized = request.LicensePlate.ToUpperInvariant().Trim();
        if (await _carRepo.IsLicensePlateExistsAsync(licensePlateNormalized, ct))
            return Result<CreateCarResponse>.Failure(
                Error.Conflict("Car.LicensePlateExists", $"Biển số '{licensePlateNormalized}' đã được đăng ký."));

        var car = Car.Create(userId.Value, request.LicensePlate, request.Brand, request.Model, request.ManufacturingYear);

        await _carRepo.AddAsync(car, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<CreateCarResponse>.Success(new CreateCarResponse(
            car.Id, car.LicensePlate, car.Brand, car.Model, car.ManufacturingYear));
    }
}
