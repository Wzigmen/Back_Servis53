namespace UserManagerApi.DTO;

public class ProductCreateDto
{
    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int? WarrantyMonths { get; set; }
}
