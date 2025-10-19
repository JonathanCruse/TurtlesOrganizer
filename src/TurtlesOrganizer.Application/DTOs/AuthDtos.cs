namespace TurtlesOrganizer.Application.DTOs;

public record RegisterUserDto(string FullName, string Email, string Password);

public record LoginDto(string Email, string Password);

public record UserDto(Guid Id, string FullName, string Email, DateTime CreatedAt);
