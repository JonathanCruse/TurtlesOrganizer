using TurtlesOrganizer.Application.DTOs;
using TurtlesOrganizer.Domain.Entities;
using TurtlesOrganizer.Domain.Repositories;
using TurtlesOrganizer.Domain.ValueObjects;

namespace TurtlesOrganizer.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
    {
        var email = Email.Create(dto.Email);
        
        if (await _userRepository.ExistsAsync(email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Simple password hashing (in production, use BCrypt or similar)
        var passwordHash = HashPassword(dto.Password);
        
        var user = new User(dto.FullName, email, passwordHash);
        await _userRepository.AddAsync(user);

        return MapToDto(user);
    }

    public async Task<UserDto?> LoginAsync(LoginDto dto)
    {
        var email = Email.Create(dto.Email);
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return null;

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            return null;

        if (!user.IsActive)
            throw new InvalidOperationException("User account is deactivated");

        return MapToDto(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    private UserDto MapToDto(User user) => new UserDto(
        user.Id,
        user.FullName,
        user.Email.Value,
        user.CreatedAt
    );

    private string HashPassword(string password)
    {
        // Simple hashing for now - in production use BCrypt.Net or ASP.NET Core Identity
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var computedHash = HashPassword(password);
        return computedHash == hash;
    }
}
