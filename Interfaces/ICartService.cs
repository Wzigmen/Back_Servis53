using UserManagerApi.DTO;

namespace UserManagerApi.Interfaces;

public interface ICartService
{
    // Возвращает текст ошибки или null при успехе
    Task<string?> AddToCartAsync(int userId, AddToCartDto dto);
    Task<CartDto> GetCartAsync(int userId);
    Task<string?> UpdateQuantityAsync(int userId, int productId, int quantity);
    Task RemoveAsync(int userId, int productId);
}
