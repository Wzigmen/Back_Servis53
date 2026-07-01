using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("news")]
public class News
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = "";

    [Column("content")]
    public string? Content { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }
}