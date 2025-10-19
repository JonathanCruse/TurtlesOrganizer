using TurtlesOrganizer.Domain.Common;
using TurtlesOrganizer.Domain.ValueObjects;

namespace TurtlesOrganizer.Domain.Entities;

public class Person : Entity
{
    public string FullName { get; private set; }
    public Email Email { get; private set; }
    public Guid? MembershipId { get; private set; }
    public bool IsTrainer { get; private set; }
    public bool IsMember => MembershipId.HasValue;
    public bool IsGuest => !MembershipId.HasValue;

    private Person() { } // For EF Core

    public Person(string fullName, Email email, Guid? membershipId = null, bool isTrainer = false)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        MembershipId = membershipId;
        IsTrainer = isTrainer;
    }

    public void AssignMembership(Guid membershipId)
    {
        MembershipId = membershipId;
    }

    public void SetTrainerStatus(bool isTrainer)
    {
        IsTrainer = isTrainer;
    }
}
