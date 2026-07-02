using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MessagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все сообщения
    [HttpGet]
    public async Task<IActionResult> GetMessages()
    {
        var messages = await _context.Messages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return Ok(messages);
    }

    // Получить сообщение по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMessage(int id)
    {
        var message = await _context.Messages.FindAsync(id);

        if (message == null)
            return NotFound("Сообщение не найдено.");

        return Ok(message);
    }

    // Отправить сообщение
    [HttpPost]
    public async Task<IActionResult> CreateMessage(Message message)
    {
        message.CreatedAt = DateTime.UtcNow;

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessage),
            new { id = message.Id }, message);
    }

    // Изменить сообщение
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMessage(int id, Message message)
    {
        if (id != message.Id)
            return BadRequest("ID не совпадают.");

        _context.Entry(message).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Messages.AnyAsync(m => m.Id == id))
                return NotFound("Сообщение не найдено.");

            throw;
        }

        return NoContent();
    }

    // Удалить сообщение
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var message = await _context.Messages.FindAsync(id);

        if (message == null)
            return NotFound("Сообщение не найдено.");

        _context.Messages.Remove(message);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
