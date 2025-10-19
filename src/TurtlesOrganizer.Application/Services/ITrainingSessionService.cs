using TurtlesOrganizer.Application.DTOs;

namespace TurtlesOrganizer.Application.Services;

public interface ITrainingSessionService
{
    Task<TrainingSessionDto> CreateSessionAsync(CreateTrainingSessionDto dto);
    Task<IEnumerable<TrainingSessionDto>> GetUpcomingSessionsAsync();
    Task<IEnumerable<TrainingSessionDto>> GetSessionsByTrainingIdAsync(Guid trainingId);
    Task RegisterAttendeeAsync(Guid sessionId, Guid personId);
    Task UnregisterAttendeeAsync(Guid sessionId, Guid personId);
}
