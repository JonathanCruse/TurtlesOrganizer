public class Attendee
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Person
{
    public Guid PersonId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public Guid? MembershipId { get; set; }
    public bool IsMember => MembershipId.HasValue;
    public bool IsGuest => !MembershipId.HasValue;
}

public class TrainingSession
{
    public Guid SessionId { get; set; }
    public Guid TrainingId { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public Person Trainer { get; set; }
    public List<Person> Attendees { get; set; }
    public int MaxAttendees { get; set; }
    public bool IsFull => Attendees.Count >= MaxAttendees;
    public bool IsUpcoming => Date > DateTime.UtcNow;
    public int AvailableSpots => MaxAttendees - Attendees.Count;
}

public class Training
{
    public Guid TrainingId { get; set; }
    public string Topic { get; set; }
}