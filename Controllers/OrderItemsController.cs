using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Models;

namespace UserManagerApi.Controllers;

[Route("api/[controller]")]
public class OrderItemsController : CrudController<OrderItem>
{
    public OrderItemsController(ApplicationDbContext context) : base(context) { }

    protected override string NotFoundMessage => "Товар заказа не найден.";

    // Все товары определенного заказа
    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetItemsByOrder(int orderId)
    {
        var items = await Set
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .ToListAsync();

        return Ok(items);
    }
}
