using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface IInsurancePolicyRepository
{
    Task AddAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);
    Task<InsurancePolicy?> GetByIdAsync(Guid policyId, CancellationToken cancellationToken = default);
    Task<List<InsurancePolicy>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<InsurancePolicy>> GetPendingPoliciesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Kiểm tra xem xe này có đang có hợp đồng bảo hiểm nào (PendingApproval hoặc Active) không.
    /// Mỗi xe chỉ được phép có tối đa 1 hợp đồng đang hiệu lực.
    /// </summary>
    Task<bool> HasActivePolicyAsync(Guid carId, CancellationToken cancellationToken = default);
}
