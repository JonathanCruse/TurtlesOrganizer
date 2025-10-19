using Microsoft.EntityFrameworkCore;
using TurtlesOrganizer.Domain.Entities;
using TurtlesOrganizer.Domain.Repositories;
using TurtlesOrganizer.Domain.ValueObjects;
using TurtlesOrganizer.Infrastructure.Persistence;

namespace TurtlesOrganizer.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Email email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.Value == email.Value);
    }
}
