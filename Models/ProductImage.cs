using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("product_images")]
public class ProductImage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("image_name")]
    public string ImageName { get; set; } = "";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}
