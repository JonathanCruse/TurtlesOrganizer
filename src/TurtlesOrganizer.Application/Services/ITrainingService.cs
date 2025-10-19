using TurtlesOrganizer.Application.DTOs;

namespace TurtlesOrganizer.Application.Services;

public interface ITrainingService
{
    Task<TrainingDto> CreateTrainingAsync(CreateTrainingDto dto, Guid userId);
    Task<TrainingWithSessionsDto?> GetTrainingByIdAsync(Guid id);
    Task<IEnumerable<TrainingDto>> GetAllTrainingsAsync();
    Task<IEnumerable<TrainingDto>> GetUserTrainingsAsync(Guid userId);
}
