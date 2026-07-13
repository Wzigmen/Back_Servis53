using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("login")]
    public string Login { get; set; } = "";

    [Column("password")]
    public string Password { get; set; } = "";

    [Column("email")]
    public string? Email { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("full_name")]
    public string? FullName { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("avatar")]
    public string? Avatar { get; set; }
}