using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("messages")]
public class Message
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_name")]
    public string UserName { get; set; } = "";

    [Column("email")]
    public string Email { get; set; } = "";

    [Column("subject")]
    public string Subject { get; set; } = "";

    [Column("message")]
    public string MessageText { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}