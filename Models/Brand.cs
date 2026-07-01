using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("brands")]
public class Brand
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("country")]
    public string? Country { get; set; }

    [Column("logo")]
    public string? Logo { get; set; }
}
