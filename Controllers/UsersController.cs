using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ImageStorage _images;

    public UsersController(ApplicationDbContext context, ImageStorage images)
    {
        _context = context;
        _images = images;
    }

    // Все пользователи (без паролей)
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .OrderBy(x => x.Id)
            .ToListAsync();

        return Ok(users.Select(UserDto.From));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        return user == null ? NotFound("Пользователь не найден.") : Ok(UserDto.From(user));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (id == User.GetUserId())
            return BadRequest("Нельзя удалить самого себя.");

        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound("Пользователь не найден.");

        // избранное удаляем сами (в БД нет каскада), корзина удалится каскадно
        _context.Favorites.RemoveRange(_context.Favorites.Where(f => f.UserId == id));
        _context.Users.Remove(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("У пользователя есть заказы, заявки или отзывы — удалить его нельзя.");
        }

        _images.DeleteFile(_images.AvatarFolder(), user.Avatar);

        return NoContent();
    }

    [HttpPut("{id:int}/role")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] int roleId)
    {
        if (id == User.GetUserId())
            return BadRequest("Нельзя изменить собственную роль.");

        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound("Пользователь не найден.");

        if (!await _context.Roles.AnyAsync(r => r.Id == roleId))
            return BadRequest("Роль не найдена.");

        user.RoleId = roleId;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Роль изменена" });
    }

    // Редактирование своего профиля
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == User.GetUserId());

        if (user == null)
            return NotFound();

        user.FullName = dto.FullName?.Trim();
        user.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        user.Phone = dto.Phone?.Trim();

        await _context.SaveChangesAsync();

        return Ok(UserDto.From(user));
    }

    // Загрузка аватара текущим пользователем
    [HttpPost("avatar")]
    [RequestSizeLimit(ImageStorage.MaxFileSize + 1024 * 1024)]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var error = ImageStorage.Validate(file);

        if (error != null)
            return BadRequest(error);

        var user = await _context.Users.FindAsync(User.GetUserId());

        if (user == null)
            return NotFound();

        var folder = _images.AvatarFolder();

        // старый файл удаляем: при другом расширении он бы остался на диске
        _images.DeleteFile(folder, user.Avatar);

        // суффикс не даёт браузеру показывать старую картинку из кеша
        var fileName = await _images.SaveAsync(file, folder, $"{user.Id}_{DateTime.UtcNow.Ticks}");

        user.Avatar = fileName;

        await _context.SaveChangesAsync();

        return Ok(new { avatar = fileName });
    }
}
