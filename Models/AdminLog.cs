using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("adminlogs")]
public class AdminLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("admin_id")]
    public int AdminId { get; set; }

    [Column("action")]
    public string Action { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
