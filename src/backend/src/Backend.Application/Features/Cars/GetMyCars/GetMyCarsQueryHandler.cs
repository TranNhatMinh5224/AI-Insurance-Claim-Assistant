using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Cars.GetMyCars;

internal sealed class GetMyCarsQueryHandler 
    : IRequestHandler<GetMyCarsQuery, Result<List<GetMyCarsResponse>>>
{
    private readonly ICarRepository _carRepo;
    private readonly ICurrentUserService _currentUser;

    public GetMyCarsQueryHandler(ICarRepository carRepo, ICurrentUserService currentUser)
    {
        _carRepo = carRepo;
        _currentUser = currentUser;
    }

    public async Task<Result<List<GetMyCarsResponse>>> Handle(GetMyCarsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.GetUserIdOrThrow();
        var cars = await _carRepo.GetByUserIdAsync(userId, ct);

        var response = cars.Select(c => new GetMyCarsResponse(
            c.Id,
            c.LicensePlate,
            c.Brand,
            c.Model,
            c.Year,
            c.VinNumber,
            c.Status.ToString()
        )).ToList();

        return Result<List<GetMyCarsResponse>>.Success(response);
    }
}
