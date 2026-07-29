using Backend.Domain.Entities;

namespace Backend.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
