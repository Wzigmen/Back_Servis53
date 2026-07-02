using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminLogsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все действия администратора
    [HttpGet]
    public async Task<IActionResult> GetLogs()
    {
        var logs = await _context.AdminLogs
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return Ok(logs);
    }

    // Получить действие по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLog(int id)
    {
        var log = await _context.AdminLogs.FindAsync(id);

        if (log == null)
            return NotFound("Запись не найдена.");

        return Ok(log);
    }

    // Добавить запись
    [HttpPost]
    public async Task<IActionResult> CreateLog(AdminLog log)
    {
        log.CreatedAt = DateTime.UtcNow;

        _context.AdminLogs.Add(log);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLog),
            new { id = log.Id }, log);
    }

    // Изменить запись
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLog(int id, AdminLog log)
    {
        if (id != log.Id)
            return BadRequest("ID не совпадают.");

        _context.Entry(log).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.AdminLogs.AnyAsync(l => l.Id == id))
                return NotFound("Запись не найдена.");

            throw;
        }

        return NoContent();
    }

    // Удалить запись
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLog(int id)
    {
        var log = await _context.AdminLogs.FindAsync(id);

        if (log == null)
            return NotFound("Запись не найдена.");

        _context.AdminLogs.Remove(log);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}