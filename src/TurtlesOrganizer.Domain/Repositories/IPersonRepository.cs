using TurtlesOrganizer.Domain.Entities;

namespace TurtlesOrganizer.Domain.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id);
    Task<IEnumerable<Person>> GetAllAsync();
    Task AddAsync(Person person);
    Task UpdateAsync(Person person);
}
