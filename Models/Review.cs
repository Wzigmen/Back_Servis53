using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    // В таблице reviews нет колонки order_id, поэтому поля OrderId здесь нет
    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
