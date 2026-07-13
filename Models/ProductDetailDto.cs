using UserManagerApi.Models;

namespace UserManagerApi.DTO;

public class ProductDetailDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Brand { get; set; }

    public string? Category { get; set; }

    public List<string> Images { get; set; } = new();

    public PhoneSpec? Phone { get; set; }

    public LaptopSpec? Laptop { get; set; }

    public PcSpec? Pc { get; set; }

    public HeadphoneSpec? Headphones { get; set; }
}