using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepairHistoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RepairHistoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить всю историю
    [HttpGet]
    public async Task<IActionResult> GetHistory()
    {
        var history = await _context.RepairHistory
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();

        return Ok(history);
    }

    // Получить запись истории по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHistoryItem(int id)
    {
        var item = await _context.RepairHistory.FindAsync(id);

        if (item == null)
            return NotFound("Запись не найдена.");

        return Ok(item);
    }

    // Получить историю конкретного ремонта
    [HttpGet("repair/{repairId}")]
    public async Task<IActionResult> GetRepairHistory(int repairId)
    {
        var history = await _context.RepairHistory
            .Where(h => h.RepairId == repairId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();

        return Ok(history);
    }

    // Добавить запись в историю
    [HttpPost]
    public async Task<IActionResult> CreateHistory(RepairHistory history)
    {
        history.ChangedAt = DateTime.UtcNow;

        _context.RepairHistory.Add(history);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHistoryItem), new { id = history.Id }, history);
    }

    // Изменить комментарий
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHistory(int id, RepairHistory history)
    {
        if (id != history.Id)
            return BadRequest();

        _context.Entry(history).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Удалить запись истории
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHistory(int id)
    {
        var history = await _context.RepairHistory.FindAsync(id);

        if (history == null)
            return NotFound();

        _context.RepairHistory.Remove(history);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}