using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RepairsController : ControllerBase
{
    public static readonly string[] Statuses = ["Принята", "В работе", "Готово", "Отменена"];

    private readonly ApplicationDbContext _context;

    public RepairsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Все заявки (админка). Для гостевых заявок берём контакты из самой заявки.
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetRepairs()
    {
        var repairs = await _context.Repairs
            .AsNoTracking()
            .OrderByDescending(r => r.DateCreated)
            .Select(r => new
            {
                r.Id,
                r.UserId,
                UserName = r.ClientName ?? r.User!.FullName,
                UserPhone = r.ClientPhone ?? r.User!.Phone,
                UserEmail = r.ClientEmail ?? r.User!.Email,
                r.DeviceType,
                r.Brand,
                r.Model,
                r.Problem,
                r.Status,
                r.Price,
                r.DateCreated,
                r.DateFinished
            })
            .ToListAsync();

        return Ok(repairs);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRepair(int id)
    {
        var repair = await _context.Repairs.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

        if (repair == null || (repair.UserId != User.GetUserId() && !User.IsAdmin()))
            return NotFound("Заявка не найдена.");

        return Ok(repair);
    }

    // Заявки текущего пользователя
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRepairs()
    {
        var userId = User.GetUserId();

        var repairs = await _context.Repairs
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.DateCreated)
            .ToListAsync();

        return Ok(repairs);
    }

    // Создать заявку на ремонт — доступно и гостям
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateRepair(RepairCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ClientPhone) && User.GetUserId() == null)
            return BadRequest("Укажите телефон для связи.");

        var repair = new Repair
        {
            UserId = User.GetUserId(),
            ClientName = dto.ClientName,
            ClientPhone = dto.ClientPhone,
            ClientEmail = string.IsNullOrWhiteSpace(dto.ClientEmail) ? null : dto.ClientEmail,
            DeviceType = dto.DeviceType,
            Brand = dto.Brand,
            Model = dto.Model,
            Problem = dto.Problem,
            Status = Statuses[0],
            DateCreated = DateTime.UtcNow
        };

        _context.Repairs.Add(repair);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRepair), new { id = repair.Id }, new { repair.Id, repair.Status });
    }

    // Изменить всю заявку
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateRepair(int id, Repair repair)
    {
        if (id != repair.Id)
            return BadRequest("ID не совпадают.");

        if (!await _context.Repairs.AnyAsync(r => r.Id == id))
            return NotFound("Заявка не найдена.");

        _context.Entry(repair).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Изменить статус заявки (с записью в историю)
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        if (!Statuses.Contains(status))
            return BadRequest("Неизвестный статус.");

        var repair = await _context.Repairs.FindAsync(id);

        if (repair == null)
            return NotFound("Заявка не найдена.");

        repair.Status = status;
        repair.DateFinished = status == "Готово" ? DateTime.UtcNow : null;

        _context.RepairHistory.Add(new RepairHistory
        {
            RepairId = id,
            Status = status,
            ChangedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(new { repair.Id, repair.Status, repair.DateFinished });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteRepair(int id)
    {
        var repair = await _context.Repairs.FindAsync(id);

        if (repair == null)
            return NotFound("Заявка не найдена.");

        _context.RepairHistory.RemoveRange(_context.RepairHistory.Where(h => h.RepairId == id));
        _context.Repairs.Remove(repair);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
