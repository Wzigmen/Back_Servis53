using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(ApplicationDbContext context,
                       IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var exists = await _context.Users
            .AnyAsync(x => x.Login == dto.Login);

        if (exists)
            return false;

        var userRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == "User");

        if (userRole == null)
            throw new Exception("Роль User отсутствует в базе.");

        var user = new User
        {
            Login = dto.Login,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Email = dto.Email,
            Phone = dto.Phone,
            FullName = dto.FullName,
            RoleId = userRole.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(bool Success, string Token, UserDto? User)> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == dto.Login);

        if (user == null)
            return (false, "", null);

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return (false, "", null);

        user.LastLogin = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user, user.Role!.RoleName);

        var userDto = new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email ?? "",
            FullName = user.FullName ?? "",
            Role = user.Role.RoleName
        };

        return (true, token, userDto);
    }
    public async Task<UserDto?> GetCurrentUserAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email ?? "",
            FullName = user.FullName ?? "",
            Role = user.Role!.RoleName
        };
    }
}