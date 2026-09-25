using System.ComponentModel.DataAnnotations;

namespace UserManagerApi.DTO;

// Используется для создания и редактирования товара
public class ProductCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию.")]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Выберите бренд.")]
    public int BrandId { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    [Range(0, 100_000_000)]
    public decimal Price { get; set; }

    [Range(0, 1_000_000)]
    public int Quantity { get; set; }

    [Range(0, 600)]
    public int? WarrantyMonths { get; set; }
}
