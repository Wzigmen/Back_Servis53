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
    [HttpGet]
    public async Task<IActionResult> GetFavorites()
    {
        var favorites = await _context.Favorites.ToListAsync();
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
    public async Task<IActionResult> AddFavorite(Favorite favorite)
    {
        bool exists = await _context.Favorites.AnyAsync(f =>
            f.UserId == favorite.UserId &&
            f.ProductId == favorite.ProductId);

        if (exists)
            return BadRequest("Товар уже находится в избранном.");

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok(favorite);
    }

    // Удалить товар из избранного
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFavorite(int id)
    {
        var favorite = await _context.Favorites.FindAsync(id);

        if (favorite == null)
            return NotFound();

        _context.Favorites.Remove(favorite);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
