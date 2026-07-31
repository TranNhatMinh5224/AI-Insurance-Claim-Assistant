using Backend.Application.Abstractions;
using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

internal sealed class CustomerDocumentRepository : ICustomerDocumentRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerDocumentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(CustomerDocument document, CancellationToken cancellationToken = default)
        => await _dbContext.CustomerDocuments.AddAsync(document, cancellationToken);

    public async Task<CustomerDocument?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
        => await _dbContext.CustomerDocuments.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

    public async Task<List<CustomerDocument>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbContext.CustomerDocuments
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
}
