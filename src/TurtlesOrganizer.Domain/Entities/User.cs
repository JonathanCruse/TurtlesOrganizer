using TurtlesOrganizer.Domain.Common;
using TurtlesOrganizer.Domain.ValueObjects;

namespace TurtlesOrganizer.Domain.Entities;

public class User : Entity
{
    public string FullName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // For EF Core

    public User(string fullName, Email email, string passwordHash)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdateProfile(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));
        
        FullName = fullName;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
