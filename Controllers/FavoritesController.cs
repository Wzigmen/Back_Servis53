using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoritesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все избранные товары
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        var favorites = await _context.Favorites
            .Include(x => x.Product)
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return Ok(favorites);
    }

    // Получить избранное пользователя
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserFavorites(int userId)
    {
        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .ToListAsync();

        return Ok(favorites);
    }

    // Добавить товар в избранное
    [HttpPost]
    public async Task<IActionResult> Add(Favorite favorite)
    {
        var exists = await _context.Favorites.AnyAsync(x =>
            x.UserId == favorite.UserId &&
            x.ProductId == favorite.ProductId);

        if (exists)
            return BadRequest("Уже в избранном");

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // Удалить товар из избранного
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var favorite = await _context.Favorites.FindAsync(id);

        if (favorite == null)
            return NotFound();

        _context.Favorites.Remove(favorite);

        await _context.SaveChangesAsync();

        return Ok();
    }
}
