using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UserManagerApi.Models;

[Table("orderitems")]
public class OrderItem : IEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [JsonIgnore, ValidateNever]
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    [JsonIgnore, ValidateNever]
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;
}
