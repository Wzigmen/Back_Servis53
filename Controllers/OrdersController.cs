using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Helpers;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    public static readonly string[] Statuses = ["Новый", "В работе", "Выполнен", "Отменён"];

    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int UserId => User.GetUserId()!.Value;

    // Все заказы (админка)
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.TotalPrice,
                x.OrderDate,
                x.UserId,
                UserName = x.User!.FullName,
                UserPhone = x.User.Phone,
                UserEmail = x.User.Email
            })
            .ToListAsync();

        return Ok(orders);
    }

    // Заказы текущего пользователя вместе с товарами
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = UserId;

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.TotalPrice,
                x.OrderDate,
                Items = _context.OrderItems
                    .Where(i => i.OrderId == x.Id)
                    .Select(i => new
                    {
                        i.ProductId,
                        ProductName = i.Product.Name,
                        i.Quantity,
                        i.Price
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    // Подробности заказа: владелец или админ
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null || (order.UserId != UserId && !User.IsAdmin()))
            return NotFound("Заказ не найден.");

        var items = await _context.OrderItems
            .AsNoTracking()
            .Where(i => i.OrderId == id)
            .Select(i => new
            {
                i.ProductId,
                ProductName = i.Product.Name,
                i.Quantity,
                i.Price
            })
            .ToListAsync();

        return Ok(new
        {
            order.Id,
            order.Status,
            order.TotalPrice,
            order.OrderDate,
            FullName = order.User?.FullName,
            Email = order.User?.Email,
            Phone = order.User?.Phone,
            Items = items
        });
    }

    // Оформить заказ из корзины текущего пользователя
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var userId = UserId;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var cartItems = await _context.CartItems
            .Include(x => x.Product)
            .Where(x => x.Cart.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            return BadRequest("Корзина пуста.");

        var notEnough = cartItems.FirstOrDefault(x => x.Quantity > x.Product.Quantity);

        if (notEnough != null)
            return BadRequest($"Товара «{notEnough.Product.Name}» в наличии только {notEnough.Product.Quantity} шт.");

        var order = new Order
        {
            UserId = userId,
            Status = Statuses[0],
            OrderDate = DateTime.UtcNow,
            TotalPrice = cartItems.Sum(x => x.Quantity * x.Product.Price)
        };

        _context.Orders.Add(order);

        foreach (var item in cartItems)
        {
            _context.OrderItems.Add(new OrderItem
            {
                Order = order,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Product.Price
            });

            // списываем со склада
            item.Product.Quantity -= item.Quantity;
        }

        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new { order.Id, order.Status, order.TotalPrice, order.OrderDate });
    }

    // Изменить статус заказа
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        if (!Statuses.Contains(status))
            return BadRequest("Неизвестный статус.");

        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound("Заказ не найден.");

        order.Status = status;

        await _context.SaveChangesAsync();

        return Ok(new { order.Id, order.Status });
    }

    // Удалить заказ вместе с позициями
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound("Заказ не найден.");

        _context.OrderItems.RemoveRange(_context.OrderItems.Where(x => x.OrderId == id));
        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
