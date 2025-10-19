using Microsoft.EntityFrameworkCore;
using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;
using TurtlesOrganizer.Domain.Repositories;
using TurtlesOrganizer.Infrastructure.Persistence;

namespace TurtlesOrganizer.Infrastructure.Repositories;

public class TrainingRepository : ITrainingRepository
{
    private readonly ApplicationDbContext _context;

    public TrainingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Training?> GetByIdAsync(Guid id)
    {
        return await _context.Trainings.FindAsync(id);
    }

    public async Task<Training?> GetByIdWithSessionsAsync(Guid id)
    {
        // Use Entry API to load the collection through the backing field
        var training = await _context.Trainings.FirstOrDefaultAsync(t => t.Id == id);
        if (training != null)
        {
            await _context.Entry(training)
                .Collection(nameof(Training.Sessions))
                .LoadAsync();
        }
        return training;
    }

    public async Task<IEnumerable<Training>> GetAllAsync()
    {
        var trainings = await _context.Trainings.ToListAsync();
        foreach (var training in trainings)
        {
            await _context.Entry(training)
                .Collection(nameof(Training.Sessions))
                .LoadAsync();
        }
        return trainings;
    }

    public async Task<IEnumerable<Training>> GetByUserIdAsync(Guid userId)
    {
        var trainings = await _context.Trainings
            .Where(t => t.CreatedByUserId == userId)
            .ToListAsync();
        
        foreach (var training in trainings)
        {
            await _context.Entry(training)
                .Collection(nameof(Training.Sessions))
                .LoadAsync();
        }
        return trainings;
    }

    public async Task AddAsync(Training training)
    {
        await _context.Trainings.AddAsync(training);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Training training)
    {
        _context.Trainings.Update(training);
        await _context.SaveChangesAsync();
    }
}
