using JwtBackendApi.Models;

namespace JwtBackendApi.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponse> LoginAsync(LoginDto loginDto);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int userId);
    Task<List<User>> GetAllUsersAsync();
}
