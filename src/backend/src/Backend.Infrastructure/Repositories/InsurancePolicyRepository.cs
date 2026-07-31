using Backend.Application.Abstractions;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

internal sealed class InsurancePolicyRepository : IInsurancePolicyRepository
{
    private readonly AppDbContext _dbContext;

    public InsurancePolicyRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(InsurancePolicy policy, CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePolicies.AddAsync(policy, cancellationToken);

    public async Task<InsurancePolicy?> GetByIdAsync(Guid policyId, CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePolicies.FirstOrDefaultAsync(p => p.Id == policyId, cancellationToken);

    public async Task<bool> HasActivePolicyAsync(Guid carId, CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePolicies.AnyAsync(
            p => p.CarId == carId && (p.Status == PolicyStatus.Active || p.Status == PolicyStatus.PendingApproval),
            cancellationToken);

    public async Task<List<InsurancePolicy>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePolicies
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<InsurancePolicy>> GetPendingPoliciesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.InsurancePolicies
            .Where(p => p.Status == PolicyStatus.PendingApproval)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
}
