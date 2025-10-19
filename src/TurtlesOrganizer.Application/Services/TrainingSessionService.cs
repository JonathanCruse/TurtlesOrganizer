using TurtlesOrganizer.Application.DTOs;
using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;
using TurtlesOrganizer.Domain.Repositories;

namespace TurtlesOrganizer.Application.Services;

public class TrainingSessionService : ITrainingSessionService
{
    private readonly ITrainingSessionRepository _sessionRepository;
    private readonly ITrainingRepository _trainingRepository;

    public TrainingSessionService(
        ITrainingSessionRepository sessionRepository,
        ITrainingRepository trainingRepository)
    {
        _sessionRepository = sessionRepository;
        _trainingRepository = trainingRepository;
    }

    public async Task<TrainingSessionDto> CreateSessionAsync(CreateTrainingSessionDto dto)
    {
        var training = await _trainingRepository.GetByIdAsync(dto.TrainingId);
        if (training == null)
            throw new InvalidOperationException("Training not found");

        var session = training.AddSession(dto.Title, dto.Date, dto.TrainerId, dto.MaxAttendees);
        await _trainingRepository.UpdateAsync(training);
        
        return MapToDto(session);
    }

    public async Task<IEnumerable<TrainingSessionDto>> GetUpcomingSessionsAsync()
    {
        var sessions = await _sessionRepository.GetUpcomingSessionsAsync();
        return sessions.Select(MapToDto);
    }

    public async Task<IEnumerable<TrainingSessionDto>> GetSessionsByTrainingIdAsync(Guid trainingId)
    {
        var sessions = await _sessionRepository.GetByTrainingIdAsync(trainingId);
        return sessions.Select(MapToDto);
    }

    public async Task RegisterAttendeeAsync(Guid sessionId, Guid personId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new InvalidOperationException("Training session not found");

        session.RegisterAttendee(personId);
        await _sessionRepository.UpdateAsync(session);
    }

    public async Task UnregisterAttendeeAsync(Guid sessionId, Guid personId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new InvalidOperationException("Training session not found");

        session.UnregisterAttendee(personId);
        await _sessionRepository.UpdateAsync(session);
    }

    private TrainingSessionDto MapToDto(TrainingSession session) => new TrainingSessionDto(
        session.Id,
        session.TrainingId,
        session.Title,
        session.Date,
        session.TrainerId,
        session.MaxAttendees,
        session.AttendeeIds.Count,
        session.AvailableSpots,
        session.IsFull,
        session.IsUpcoming
    );
}
