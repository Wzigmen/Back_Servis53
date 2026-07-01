using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("total_price")]
    public decimal TotalPrice { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; }
}