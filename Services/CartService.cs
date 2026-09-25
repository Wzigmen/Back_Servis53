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

    public async Task<string?> AddToCartAsync(int userId, AddToCartDto dto)
    {
        if (dto.Quantity <= 0)
            return "Количество должно быть больше нуля.";

        var product = await _context.Products.FindAsync(dto.ProductId);

        if (product == null)
            return "Товар не найден.";

        var cart = await GetOrCreateCartAsync(userId);

        var item = cart.Items.FirstOrDefault(x => x.ProductId == dto.ProductId);

        var newQuantity = (item?.Quantity ?? 0) + dto.Quantity;

        if (newQuantity > product.Quantity)
            return $"В наличии только {product.Quantity} шт.";

        if (item != null)
        {
            item.Quantity = newQuantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }

        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var items = await _context.CartItems
            .AsNoTracking()
            .Where(x => x.Cart.UserId == userId)
            .OrderBy(x => x.Id)
            .Select(x => new CartItemDto
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
            })
            .ToListAsync();

        return new CartDto
        {
            Items = items,
            Total = items.Sum(x => x.Price * x.Quantity)
        };
    }

    public async Task<string?> UpdateQuantityAsync(int userId, int productId, int quantity)
    {
        var item = await FindItemAsync(userId, productId);

        if (item == null)
            return null;

        if (quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }
        else
        {
            if (quantity > item.Product.Quantity)
                return $"В наличии только {item.Product.Quantity} шт.";

            item.Quantity = quantity;
        }

        await _context.SaveChangesAsync();

        return null;
    }

    public async Task RemoveAsync(int userId, int productId)
    {
        var item = await FindItemAsync(userId, productId);

        if (item == null)
            return;

        _context.CartItems.Remove(item);

        await _context.SaveChangesAsync();
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cart != null)
            return cart;

        cart = new Cart { UserId = userId };

        _context.Carts.Add(cart);

        return cart;
    }

    private Task<CartItem?> FindItemAsync(int userId, int productId) =>
        _context.CartItems
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Cart.UserId == userId && x.ProductId == productId);
}
