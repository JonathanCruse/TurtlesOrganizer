using TurtlesOrganizer.Application.DTOs;
using TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;
using TurtlesOrganizer.Domain.Repositories;

namespace TurtlesOrganizer.Application.Services;

public class TrainingService : ITrainingService
{
    private readonly ITrainingRepository _trainingRepository;

    public TrainingService(ITrainingRepository trainingRepository)
    {
        _trainingRepository = trainingRepository;
    }

    public async Task<TrainingDto> CreateTrainingAsync(CreateTrainingDto dto, Guid userId)
    {
        var training = new Training(dto.Topic, dto.Description, userId);
        await _trainingRepository.AddAsync(training);
        return MapToDto(training);
    }

    public async Task<TrainingWithSessionsDto?> GetTrainingByIdAsync(Guid id)
    {
        var training = await _trainingRepository.GetByIdWithSessionsAsync(id);
        return training != null ? MapToDetailedDto(training) : null;
    }

    public async Task<IEnumerable<TrainingDto>> GetAllTrainingsAsync()
    {
        var trainings = await _trainingRepository.GetAllAsync();
        return trainings.Select(MapToDto);
    }

    public async Task<IEnumerable<TrainingDto>> GetUserTrainingsAsync(Guid userId)
    {
        var trainings = await _trainingRepository.GetByUserIdAsync(userId);
        return trainings.Select(MapToDto);
    }

    private TrainingDto MapToDto(Training training) => new TrainingDto(
        training.Id,
        training.Topic,
        training.Description,
        training.CreatedByUserId,
        training.CreatedAt
    );

    private TrainingWithSessionsDto MapToDetailedDto(Training training) => new TrainingWithSessionsDto(
        training.Id,
        training.Topic,
        training.Description,
        training.CreatedByUserId,
        training.CreatedAt,
        training.Sessions.Select(s => new TrainingSessionDto(
            s.Id,
            s.TrainingId,
            s.Title,
            s.Date,
            s.TrainerId,
            s.MaxAttendees,
            s.AttendeeIds.Count,
            s.AvailableSpots,
            s.IsFull,
            s.IsUpcoming
        )).ToList()
    );
}
