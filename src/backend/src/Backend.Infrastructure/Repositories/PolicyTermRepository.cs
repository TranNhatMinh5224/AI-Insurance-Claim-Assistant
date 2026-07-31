using Backend.Application.Abstractions;
using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

internal sealed class PolicyTermRepository : IPolicyTermRepository
{
    private readonly AppDbContext _dbContext;

    public PolicyTermRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(PolicyTerm policyTerm, CancellationToken cancellationToken = default)
        => await _dbContext.PolicyTerms.AddAsync(policyTerm, cancellationToken);

    public async Task<PolicyTerm?> GetByIdAsync(Guid policyTermId, CancellationToken cancellationToken = default)
        => await _dbContext.PolicyTerms.FirstOrDefaultAsync(p => p.Id == policyTermId, cancellationToken);

    public async Task<List<PolicyTerm>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
        => await _dbContext.PolicyTerms
            .Where(p => p.PackageId == packageId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<PolicyTerm?> GetCurrentByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
        => await _dbContext.PolicyTerms
            .FirstOrDefaultAsync(p => p.PackageId == packageId && p.IsCurrent, cancellationToken);
}
