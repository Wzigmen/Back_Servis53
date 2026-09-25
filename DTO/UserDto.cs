using UserManagerApi.Models;

namespace UserManagerApi.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? FullName { get; set; }
    public string Role { get; set; } = "";
    public int? RoleId { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }

    // Пароль (хеш) наружу никогда не отдаём
    public static UserDto From(User user) => new()
    {
        Id = user.Id,
        Login = user.Login,
        Email = user.Email,
        Phone = user.Phone,
        FullName = user.FullName,
        Role = user.Role?.RoleName ?? "",
        RoleId = user.RoleId,
        Avatar = user.Avatar,
        CreatedAt = user.CreatedAt
    };
}
