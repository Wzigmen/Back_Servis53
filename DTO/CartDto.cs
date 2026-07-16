namespace UserManagerApi.DTO;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}