using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly ICategoryService _service;
    public CategoriesController(
    ApplicationDbContext context,
    ICategoryService service)
    {
        _context = context;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAllAsync());
    }

    //// Получить все категории
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    //{
    //    return await _context.Categories.ToListAsync();
    //}

    // Получить категорию по ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound("Категория не найдена.");

        return category;
    }

    // Создать категорию
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    // Изменить категорию
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category category)
    {
        if (id != category.Id)
            return BadRequest("ID не совпадают.");

        _context.Entry(category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CategoryExists(id))
                return NotFound("Категория не найдена.");

            throw;
        }

        return NoContent();
    }

    // Удалить категорию
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound("Категория не найдена.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CategoryExists(int id)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id);
    }
}
