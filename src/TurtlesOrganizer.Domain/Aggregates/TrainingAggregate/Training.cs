using TurtlesOrganizer.Domain.Common;

namespace TurtlesOrganizer.Domain.Aggregates.TrainingAggregate;

public class Training : Entity
{
    public string Topic { get; private set; }
    public string? Description { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<TrainingSession> _sessions = new();
    public IReadOnlyCollection<TrainingSession> Sessions => _sessions.AsReadOnly();

    private Training() { } // For EF Core

    public Training(string topic, string? description, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic cannot be empty", nameof(topic));

        Topic = topic;
        Description = description;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public TrainingSession AddSession(string title, DateTime date, Guid trainerId, int maxAttendees)
    {
        var session = new TrainingSession(Id, title, date, trainerId, maxAttendees);
        _sessions.Add(session);
        return session;
    }

    public void UpdateDetails(string topic, string? description)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic cannot be empty", nameof(topic));

        Topic = topic;
        Description = description;
    }
}
