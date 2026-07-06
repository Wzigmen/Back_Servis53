using UserManagerApi.DTO;

namespace UserManagerApi.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string Token, UserDto? User)> LoginAsync(LoginDto dto);
    Task<UserDto?> GetCurrentUserAsync(int id);
}
