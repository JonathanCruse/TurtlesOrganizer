using TurtlesOrganizer.Domain.Entities;
using TurtlesOrganizer.Domain.ValueObjects;

namespace TurtlesOrganizer.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(Email email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsAsync(Email email);
}
