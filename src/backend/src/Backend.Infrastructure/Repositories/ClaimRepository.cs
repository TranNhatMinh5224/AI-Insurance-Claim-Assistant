using Backend.Application.Abstractions;
using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

internal sealed class ClaimRepository : IClaimRepository
{
    private readonly AppDbContext _dbContext;

    public ClaimRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task AddRequestAsync(ClaimRequest request, CancellationToken cancellationToken = default)
        => await _dbContext.ClaimRequests.AddAsync(request, cancellationToken);

    public async Task AddEvidenceAsync(ClaimEvidence evidence, CancellationToken cancellationToken = default)
        => await _dbContext.ClaimEvidences.AddAsync(evidence, cancellationToken);

    public async Task<ClaimRequest?> GetByIdAsync(Guid claimId, CancellationToken cancellationToken = default)
        => await _dbContext.ClaimRequests.FirstOrDefaultAsync(c => c.Id == claimId, cancellationToken);

    public async Task<List<ClaimRequest>> GetByPolicyIdAsync(Guid policyId, CancellationToken cancellationToken = default)
        => await _dbContext.ClaimRequests
            .Where(c => c.InsurancePolicyId == policyId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<ClaimRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbContext.ClaimRequests
            .Join(_dbContext.InsurancePolicies, 
                  claim => claim.InsurancePolicyId, 
                  policy => policy.Id, 
                  (claim, policy) => new { claim, policy })
            .Where(joined => joined.policy.UserId == userId)
            .Select(joined => joined.claim)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<ClaimRequest>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.ClaimRequests
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<ClaimEvidence>> GetEvidencesByClaimIdAsync(Guid claimId, CancellationToken cancellationToken = default)
        => await _dbContext.ClaimEvidences
            .Where(e => e.ClaimRequestId == claimId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
}
