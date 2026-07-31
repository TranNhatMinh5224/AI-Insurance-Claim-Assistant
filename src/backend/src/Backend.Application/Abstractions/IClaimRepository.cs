using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface IClaimRepository
{
    Task AddRequestAsync(ClaimRequest request, CancellationToken cancellationToken = default);
    Task AddEvidenceAsync(ClaimEvidence evidence, CancellationToken cancellationToken = default);
    Task<ClaimRequest?> GetByIdAsync(Guid claimId, CancellationToken cancellationToken = default);
    Task<List<ClaimRequest>> GetByPolicyIdAsync(Guid policyId, CancellationToken cancellationToken = default);
    Task<List<ClaimRequest>> GetByPolicyIdAsync(Guid policyId, CancellationToken cancellationToken = default);
    Task<List<ClaimRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<ClaimRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<ClaimEvidence>> GetEvidencesByClaimIdAsync(Guid claimId, CancellationToken cancellationToken = default);
}
