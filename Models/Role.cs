using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("roles")]
public class Role : IEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("role_name")]
    public string RoleName { get; set; } = "";
}
