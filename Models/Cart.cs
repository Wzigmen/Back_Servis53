using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("carts")]
public class Cart
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}