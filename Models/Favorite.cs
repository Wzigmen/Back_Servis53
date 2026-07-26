using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("favorites")]
public class Favorite
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}