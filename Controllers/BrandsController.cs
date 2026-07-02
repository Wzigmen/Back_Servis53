using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BrandsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все бренды
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
    {
        return await _context.Brands.ToListAsync();
    }

    // Получить бренд по ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Brand>> GetBrand(int id)
    {
        var brand = await _context.Brands.FindAsync(id);

        if (brand == null)
            return NotFound("Бренд не найден.");

        return brand;
    }

    // Создать бренд
    [HttpPost]
    public async Task<ActionResult<Brand>> CreateBrand(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
    }

    // Изменить бренд
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBrand(int id, Brand brand)
    {
        if (id != brand.Id)
            return BadRequest("ID не совпадают.");

        _context.Entry(brand).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await BrandExists(id))
                return NotFound("Бренд не найден.");

            throw;
        }

        return NoContent();
    }

    // Удалить бренд
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var brand = await _context.Brands.FindAsync(id);

        if (brand == null)
            return NotFound("Бренд не найден.");

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> BrandExists(int id)
    {
        return await _context.Brands.AnyAsync(b => b.Id == id);
    }
}
