using Microsoft.EntityFrameworkCore;
using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;
using TurtlesOrganizer.Domain.Repositories;
using TurtlesOrganizer.Infrastructure.Persistence;

namespace TurtlesOrganizer.Infrastructure.Repositories;

public class TrainingSessionRepository : ITrainingSessionRepository
{
    private readonly ApplicationDbContext _context;

    public TrainingSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TrainingSession?> GetByIdAsync(Guid id)
    {
        return await _context.TrainingSessions.FindAsync(id);
    }

    public async Task<IEnumerable<TrainingSession>> GetByTrainingIdAsync(Guid trainingId)
    {
        return await _context.TrainingSessions
            .Where(s => s.TrainingId == trainingId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingSession>> GetUpcomingSessionsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.TrainingSessions
            .Where(s => s.Date > now)
            .OrderBy(s => s.Date)
            .ToListAsync();
    }

    public async Task AddAsync(TrainingSession session)
    {
        await _context.TrainingSessions.AddAsync(session);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TrainingSession session)
    {
        _context.TrainingSessions.Update(session);
        await _context.SaveChangesAsync();
    }
}
