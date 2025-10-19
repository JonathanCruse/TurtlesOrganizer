using TurtlesOrganizer.Domain.Common;

namespace TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;

public class TrainingSession : Entity
{
    public Guid TrainingId { get; private set; }
    public string Title { get; private set; }
    public DateTime Date { get; private set; }
    public Guid TrainerId { get; private set; }
    public int MaxAttendees { get; private set; }
    private readonly List<Guid> _attendeeIds = new();
    public IReadOnlyCollection<Guid> AttendeeIds => _attendeeIds.AsReadOnly();

    public bool IsFull => _attendeeIds.Count >= MaxAttendees;
    public bool IsUpcoming => Date > DateTime.UtcNow;
    public int AvailableSpots => MaxAttendees - _attendeeIds.Count;

    private TrainingSession() { } // For EF Core

    public TrainingSession(Guid trainingId, string title, DateTime date, Guid trainerId, int maxAttendees)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (maxAttendees <= 0)
            throw new ArgumentException("Max attendees must be greater than zero", nameof(maxAttendees));

        TrainingId = trainingId;
        Title = title;
        Date = date;
        TrainerId = trainerId;
        MaxAttendees = maxAttendees;
    }

    public void RegisterAttendee(Guid personId)
    {
        if (IsFull)
            throw new InvalidOperationException("Training session is full");

        if (_attendeeIds.Contains(personId))
            throw new InvalidOperationException("Person is already registered");

        _attendeeIds.Add(personId);
    }

    public void UnregisterAttendee(Guid personId)
    {
        if (!_attendeeIds.Contains(personId))
            throw new InvalidOperationException("Person is not registered");

        _attendeeIds.Remove(personId);
    }

    public void UpdateDetails(string title, DateTime date, int maxAttendees)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (maxAttendees < _attendeeIds.Count)
            throw new ArgumentException("Cannot reduce max attendees below current registration count");

        Title = title;
        Date = date;
        MaxAttendees = maxAttendees;
    }
}
