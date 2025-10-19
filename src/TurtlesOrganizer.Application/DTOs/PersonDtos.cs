namespace TurtlesOrganizer.Application.DTOs;

public record CreatePersonDto(string FullName, string Email, Guid? MembershipId, bool IsTrainer = false);

public record PersonDto(Guid Id, string FullName, string Email, Guid? MembershipId, bool IsMember, bool IsTrainer);
