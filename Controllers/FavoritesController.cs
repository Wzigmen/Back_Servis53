using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

// Избранное текущего пользователя (id берётся из токена)
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoritesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int UserId => User.GetUserId()!.Value;

    // Избранные товары в том же формате, что и список магазина
    [HttpGet]
    public async Task<IActionResult> GetFavorites()
    {
        var products = await _context.Favorites
            .AsNoTracking()
            .Where(x => x.UserId == UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ProductDto
            {
                Id = x.Product.Id,
                Name = x.Product.Name,
                Price = x.Product.Price,
                Quantity = x.Product.Quantity,
                Brand = x.Product.Brand!.Name,
                Category = x.Product.Category!.Name,
                Images = x.Product.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.ImageName)
                    .ToList()
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddFavoriteDto dto)
    {
        if (!await _context.Products.AnyAsync(x => x.Id == dto.ProductId))
            return NotFound("Товар не найден.");

        var exists = await _context.Favorites
            .AnyAsync(x => x.UserId == UserId && x.ProductId == dto.ProductId);

        if (exists)
            return Ok();

        _context.Favorites.Add(new Favorite
        {
            UserId = UserId,
            ProductId = dto.ProductId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Delete(int productId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(x => x.UserId == UserId && x.ProductId == productId);

        if (favorite == null)
            return NotFound();

        _context.Favorites.Remove(favorite);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
