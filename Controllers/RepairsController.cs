using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepairsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RepairsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все заявки
    [HttpGet]
    public async Task<IActionResult> GetRepairs()
    {
        var repairs = await _context.Repairs
            .OrderByDescending(r => r.DateCreated)
            .ToListAsync();

        return Ok(repairs);
    }

    // Получить заявку по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRepair(int id)
    {
        var repair = await _context.Repairs.FindAsync(id);

        if (repair == null)
            return NotFound("Заявка не найдена.");

        return Ok(repair);
    }

    // Получить заявки пользователя
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRepairs(int userId)
    {
        var repairs = await _context.Repairs
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.DateCreated)
            .ToListAsync();

        return Ok(repairs);
    }

    // Создать заявку на ремонт
    [HttpPost]
    public async Task<IActionResult> CreateRepair(Repair repair)
    {
        repair.DateCreated = DateTime.UtcNow;
        repair.Status = "Принята";

        _context.Repairs.Add(repair);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRepair), new { id = repair.Id }, repair);
    }

    // Изменить всю заявку
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRepair(int id, Repair repair)
    {
        if (id != repair.Id)
            return BadRequest();

        _context.Entry(repair).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Изменить статус заявки
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var repair = await _context.Repairs.FindAsync(id);

        if (repair == null)
            return NotFound("Заявка не найдена.");

        repair.Status = status;

        if (status == "Готово")
            repair.DateFinished = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(repair);
    }

    // Удалить заявку
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRepair(int id)
    {
        var repair = await _context.Repairs.FindAsync(id);

        if (repair == null)
            return NotFound();

        _context.Repairs.Remove(repair);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
