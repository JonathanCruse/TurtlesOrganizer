namespace TurtlesOrganizer.Application.DTOs;

public record CreateTrainingDto(string Topic, string? Description);

public record TrainingDto(Guid Id, string Topic, string? Description, Guid CreatedByUserId, DateTime CreatedAt);

public record TrainingWithSessionsDto(
    Guid Id,
    string Topic,
    string? Description,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    List<TrainingSessionDto> Sessions
);
