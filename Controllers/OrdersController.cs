using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Все заказы
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.Orders

            .Include(x => x.User)

            .Select(x => new
            {
                x.Id,
                x.Status,
                x.TotalPrice,
                x.OrderDate,

                UserId = x.UserId,
                UserName = x.User.FullName,
                UserPhone = x.User.Phone,
                UserEmail = x.User.Email
            })

            .OrderByDescending(x => x.OrderDate)

            .ToListAsync();

        return Ok(orders);
    }

    // Заказ по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound("Заказ не найден.");

        return Ok(order);
    }

    // Заказы пользователя
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserOrders(int userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(orders);
    }

    // Создать заказ
    [HttpPost]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        order.OrderDate = DateTime.UtcNow;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    // Изменить статус заказа
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound("Заказ не найден.");

        order.Status = status;

        await _context.SaveChangesAsync();

        return Ok(order);
    }

    // Обновить заказ полностью
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order order)
    {
        if (id != order.Id)
            return BadRequest();

        _context.Entry(order).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Удалить заказ
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CreateOrderDto dto)
    {
        var cartItems = await _context.CartItems
            .Include(x => x.Product)
            .Include(x => x.Cart)
            .Where(x => x.Cart.UserId == dto.UserId)
            .ToListAsync();

        if (!cartItems.Any())
            return BadRequest("Корзина пуста");

        var order = new Order
        {
            UserId = dto.UserId,
            Status = "Новый",
            OrderDate = DateTime.UtcNow,
            TotalPrice = cartItems.Sum(x => x.Quantity * x.Product!.Price)
        };

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Product!.Price
            });
        }

        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();

        return Ok(order);
    }

    [HttpGet("user/{userId}/details")]
    public async Task<IActionResult> GetUserOrdersDetails(int userId)
    {
        var orders = await _context.Orders

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
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        var items = await _context.OrderItems
            .Where(i => i.OrderId == id)
            .Include(i => i.Product)
            .Select(i => new
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                Price = i.Price
            })
            .ToListAsync();

        return Ok(new
        {
            order.Id,
            order.Status,
            order.TotalPrice,
            order.OrderDate,

            FullName = order.User.FullName,
            Email = order.User.Email,
            Phone = order.User.Phone,

            Items = items
        });
    }
}
