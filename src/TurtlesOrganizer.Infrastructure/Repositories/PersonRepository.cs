using Microsoft.EntityFrameworkCore;
using TurtlesOrganizer.Domain.Entities;
using TurtlesOrganizer.Domain.Repositories;
using TurtlesOrganizer.Infrastructure.Persistence;

namespace TurtlesOrganizer.Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly ApplicationDbContext _context;

    public PersonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Person?> GetByIdAsync(Guid id)
    {
        return await _context.Persons.FindAsync(id);
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        return await _context.Persons.ToListAsync();
    }

    public async Task AddAsync(Person person)
    {
        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Person person)
    {
        _context.Persons.Update(person);
        await _context.SaveChangesAsync();
    }
}
