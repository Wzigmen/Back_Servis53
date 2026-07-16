using UserManagerApi.DTO;

namespace UserManagerApi.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(int userId, AddToCartDto dto);
    Task<CartDto> GetCartAsync(int userId);
    Task UpdateQuantityAsync(int userId, int productId, int quantity);
    Task RemoveAsync(int userId, int productId);
}