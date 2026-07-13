using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public UsersController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // Получить всех пользователей
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // Получить пользователя по ID
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound("Пользователь не найден.");

        return user;
    }

    // Создать пользователя
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Изменить пользователя
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id)
            return BadRequest("ID не совпадают.");

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UserExists(id))
                return NotFound("Пользователь не найден.");

            throw;
        }

        return NoContent();
    }

    // Удалить пользователя
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound("Пользователь не найден.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest();

        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        var extension = Path.GetExtension(file.FileName);

        var fileName = $"{userId}{extension}";

        var folder = Path.Combine(
            _environment.WebRootPath,
            "images",
            "avatars"
        );

        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.Avatar = fileName;

        await _context.SaveChangesAsync();

        // проверяем , что пользователь имеет claim
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"{claim.Type} = {claim.Value}");
        }

        return Ok(new
        {
            avatar = fileName
        });
    }

    private async Task<bool> UserExists(int id)
    {
        return await _context.Users.AnyAsync(e => e.Id == id);
    }
}