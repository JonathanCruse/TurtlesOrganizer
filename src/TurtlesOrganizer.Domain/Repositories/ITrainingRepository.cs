using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;

namespace TurtlesOrganizer.Domain.Repositories;

public interface ITrainingRepository
{
    Task<Training?> GetByIdAsync(Guid id);
    Task<Training?> GetByIdWithSessionsAsync(Guid id);
    Task<IEnumerable<Training>> GetAllAsync();
    Task<IEnumerable<Training>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Training training);
    Task UpdateAsync(Training training);
}
