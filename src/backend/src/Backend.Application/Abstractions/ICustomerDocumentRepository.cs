using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface ICustomerDocumentRepository
{
    Task AddAsync(CustomerDocument document, CancellationToken cancellationToken = default);
    Task<CustomerDocument?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<List<CustomerDocument>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
