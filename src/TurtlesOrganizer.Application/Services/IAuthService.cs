using TurtlesOrganizer.Application.DTOs;

namespace TurtlesOrganizer.Application.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterUserDto dto);
    Task<UserDto?> LoginAsync(LoginDto dto);
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
}
