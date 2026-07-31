using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface IInsurancePackageRepository
{
    Task AddAsync(InsurancePackage package, CancellationToken cancellationToken = default);
    Task<InsurancePackage?> GetByIdAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<List<InsurancePackage>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<List<InsurancePackage>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken = default);
}
