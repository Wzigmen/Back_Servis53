using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrderItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все товары заказов
    [HttpGet]
    public async Task<IActionResult> GetOrderItems()
    {
        var orderItems = await _context.OrderItems.ToListAsync();
        return Ok(orderItems);
    }

    // Получить товар заказа по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderItem(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
            return NotFound("Товар заказа не найден.");

        return Ok(orderItem);
    }

    // Получить все товары определенного заказа
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetItemsByOrder(int orderId)
    {
        var items = await _context.OrderItems
            .Where(x => x.OrderId == orderId)
            .ToListAsync();

        return Ok(items);
    }

    // Добавить товар в заказ
    [HttpPost]
    public async Task<IActionResult> CreateOrderItem(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderItem),
            new { id = orderItem.Id }, orderItem);
    }

    // Изменить товар заказа
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem)
    {
        if (id != orderItem.Id)
            return BadRequest();

        _context.Entry(orderItem).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Удалить товар из заказа
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
            return NotFound();

        _context.OrderItems.Remove(orderItem);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
