namespace TurtlesOrganizer.Application.DTOs;

public record CreateTrainingSessionDto(
    Guid TrainingId,
    string Title,
    DateTime Date,
    Guid TrainerId,
    int MaxAttendees
);

public record TrainingSessionDto(
    Guid Id,
    Guid TrainingId,
    string Title,
    DateTime Date,
    Guid TrainerId,
    int MaxAttendees,
    int CurrentAttendees,
    int AvailableSpots,
    bool IsFull,
    bool IsUpcoming
);

public record RegisterAttendeeDto(Guid SessionId, Guid PersonId);
