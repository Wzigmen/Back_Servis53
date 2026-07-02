using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все новости
    [HttpGet]
    public async Task<IActionResult> GetNews()
    {
        var news = await _context.News
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Ok(news);
    }

    // Получить новость по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNewsById(int id)
    {
        var news = await _context.News.FindAsync(id);

        if (news == null)
            return NotFound("Новость не найдена.");

        return Ok(news);
    }

    // Добавить новость
    [HttpPost]
    public async Task<IActionResult> CreateNews(News news)
    {
        news.CreatedAt = DateTime.UtcNow;

        _context.News.Add(news);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetNewsById),
            new { id = news.Id }, news);
    }

    // Изменить новость
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNews(int id, News news)
    {
        if (id != news.Id)
            return BadRequest("ID не совпадают.");

        _context.Entry(news).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.News.AnyAsync(n => n.Id == id))
                return NotFound("Новость не найдена.");

            throw;
        }

        return NoContent();
    }

    // Удалить новость
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNews(int id)
    {
        var news = await _context.News.FindAsync(id);

        if (news == null)
            return NotFound("Новость не найдена.");

        _context.News.Remove(news);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
