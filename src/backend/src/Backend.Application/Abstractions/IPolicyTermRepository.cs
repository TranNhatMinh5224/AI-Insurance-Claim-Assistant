using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface IPolicyTermRepository
{
    Task AddAsync(PolicyTerm policyTerm, CancellationToken cancellationToken = default);
    Task<PolicyTerm?> GetByIdAsync(Guid policyTermId, CancellationToken cancellationToken = default);
    Task<List<PolicyTerm>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<PolicyTerm?> GetCurrentByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);
}
