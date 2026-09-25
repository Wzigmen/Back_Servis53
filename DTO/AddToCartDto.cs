using System.ComponentModel.DataAnnotations;

namespace UserManagerApi.DTO;

// Используется и для добавления товара в корзину, и для изменения количества
public class AddToCartDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(0, 1000)]
    public int Quantity { get; set; } = 1;
}
