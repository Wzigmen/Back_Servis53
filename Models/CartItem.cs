using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("cart_items")]
public class CartItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("cart_id")]
    public int CartId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    public Cart Cart { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
