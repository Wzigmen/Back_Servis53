using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddToCartAsync(int userId, AddToCartDto dto)
    {
        // ищем корзину пользователя

        var cart = await _context.Carts

            .Include(x => x.Items)

            .FirstOrDefaultAsync(x => x.UserId == userId);

        // если нет — создаем

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();
        }

        // ищем товар в корзине

        var item = await _context.CartItems

            .FirstOrDefaultAsync(x =>

                x.CartId == cart.Id &&
                x.ProductId == dto.ProductId);

        // если уже есть

        if (item != null)
        {
            item.Quantity += dto.Quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }

        await _context.SaveChangesAsync();
    }
    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await _context.Carts

            .Include(x => x.Items)

                .ThenInclude(x => x.Product)

                    .ThenInclude(x => x.Images)

            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cart == null)
            return new CartDto();

        var dto = new CartDto();

        dto.Items = cart.Items.Select(x => new CartItemDto
        {
            Id = x.Id,

            ProductId = x.ProductId,

            Name = x.Product.Name,

            Price = x.Product.Price,

            Quantity = x.Quantity,

            Image = x.Product.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => i.ImageName)
                .FirstOrDefault()

        }).ToList();

        dto.Total = dto.Items.Sum(x => x.Price * x.Quantity);

        return dto;
    }
    public async Task UpdateQuantityAsync(
    int userId,
    int productId,
    int quantity)
    {
        var cart = await _context.Carts

            .Include(x => x.Items)

            .FirstAsync(x => x.UserId == userId);

        var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            return;

        if (quantity <= 0)
        {
            cart.Items.Remove(item);

            _context.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        await _context.SaveChangesAsync();
    }
    public async Task RemoveAsync(
    int userId,
    int productId)
    {
        var cart = await _context.Carts

            .Include(x => x.Items)

            .FirstAsync(x => x.UserId == userId);

        var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            return;

        cart.Items.Remove(item);

        _context.CartItems.Remove(item);

        await _context.SaveChangesAsync();
    }
}