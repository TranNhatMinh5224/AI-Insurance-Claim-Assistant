using Backend.Application.Abstractions;
using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

internal sealed class CarRepository : ICarRepository
{
    private readonly AppDbContext _dbContext;

    public CarRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Car car, CancellationToken cancellationToken = default)
        => await _dbContext.Cars.AddAsync(car, cancellationToken);

    public async Task<bool> IsLicensePlateExistsAsync(string licensePlate, CancellationToken cancellationToken = default)
        => await _dbContext.Cars.AnyAsync(c => c.LicensePlate == licensePlate, cancellationToken);

    public async Task<Car?> GetByIdAsync(Guid carId, CancellationToken cancellationToken = default)
        => await _dbContext.Cars.FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);

    public async Task<List<Car>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbContext.Cars
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
}
