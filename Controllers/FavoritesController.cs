using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
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
        var products = await _context.Favorites
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                id = x.Product.Id,
                name = x.Product.Name,
                description = x.Product.Description,
                price = x.Product.Price,
                quantity = x.Product.Quantity,
                warrantyMonths = x.Product.WarrantyMonths,
                brand = x.Product.Brand.Name,
                category = x.Product.Category.Name,

                images = x.Product.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.ImageName)
                    .ToList()
            })
            .ToListAsync();

        return Ok(products);
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
    public async Task<IActionResult> Add(AddFavoriteDto dto)
    {
        var exists = await _context.Favorites.AnyAsync(x =>
            x.UserId == dto.UserId &&
            x.ProductId == dto.ProductId);

        if (exists)
            return BadRequest("Уже в избранном");

        var favorite = new Favorite
        {
            UserId = dto.UserId,
            ProductId = dto.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);

        await _context.SaveChangesAsync();

        return Ok();
    }

    // Удалить товар из избранного
    [HttpDelete("{userId}/{productId}")]
    public async Task<IActionResult> Delete(int userId, int productId)
    {
        var favorite = await _context.Favorites.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.ProductId == productId);

        if (favorite == null)
            return NotFound();

        _context.Favorites.Remove(favorite);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
