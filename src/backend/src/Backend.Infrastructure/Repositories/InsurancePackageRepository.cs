using Backend.Application.Abstractions;
using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

internal sealed class InsurancePackageRepository : IInsurancePackageRepository
{
    private readonly AppDbContext _dbContext;

    public InsurancePackageRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(InsurancePackage package, CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePackages.AddAsync(package, cancellationToken);

    public async Task<InsurancePackage?> GetByIdAsync(Guid packageId, CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePackages.FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);

    public async Task<List<InsurancePackage>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePackages.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync(cancellationToken);

    public async Task<List<InsurancePackage>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePackages.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);

    public async Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePackages.AnyAsync(p => p.Name == name, cancellationToken);
}
