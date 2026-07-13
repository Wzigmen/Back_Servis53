using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("products")]
public class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("brand_id")]
    public int? BrandId { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("warranty_months")]
    public int? WarrantyMonths { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("is_featured")]
    public bool IsFeatured { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    [ForeignKey(nameof(BrandId))]
    public Brand? Brand { get; set; }

    // Галерея изображений
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    // Характеристики
    public PhoneSpec? PhoneSpec { get; set; }

    public LaptopSpec? LaptopSpec { get; set; }

    public PcSpec? PcSpec { get; set; }

    public HeadphoneSpec? HeadphoneSpec { get; set; }
}