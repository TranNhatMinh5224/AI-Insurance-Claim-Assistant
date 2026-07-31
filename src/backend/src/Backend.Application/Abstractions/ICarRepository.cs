using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface ICarRepository
{
    Task AddAsync(Car car, CancellationToken cancellationToken = default);
    Task<bool> IsLicensePlateExistsAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<Car?> GetByIdAsync(Guid carId, CancellationToken cancellationToken = default);
    Task<List<Car>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
