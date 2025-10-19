using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;

namespace TurtlesOrganizer.Domain.Repositories;

public interface ITrainingSessionRepository
{
    Task<TrainingSession?> GetByIdAsync(Guid id);
    Task<IEnumerable<TrainingSession>> GetByTrainingIdAsync(Guid trainingId);
    Task<IEnumerable<TrainingSession>> GetUpcomingSessionsAsync();
    Task AddAsync(TrainingSession session);
    Task UpdateAsync(TrainingSession session);
}
