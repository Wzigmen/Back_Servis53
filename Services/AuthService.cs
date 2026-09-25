using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(ApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var login = dto.Login.Trim();

        if (await _context.Users.AnyAsync(x => x.Login == login))
            return false;

        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == Roles.User)
            ?? throw new InvalidOperationException("Роль User отсутствует в базе.");

        _context.Users.Add(new User
        {
            Login = login,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Email = dto.Email,
            Phone = dto.Phone,
            FullName = dto.FullName,
            RoleId = userRole.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(bool Success, string Token, UserDto? User)> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == dto.Login.Trim());

        if (user == null || !user.IsActive || user.Role == null)
            return (false, "", null);

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return (false, "", null);

        user.LastLogin = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user, user.Role.RoleName);

        return (true, token, UserDto.From(user));
    }

    public async Task<UserDto?> GetCurrentUserAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        return user == null ? null : UserDto.From(user);
    }
}
