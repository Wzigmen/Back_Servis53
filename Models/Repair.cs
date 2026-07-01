using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("repairs")]
public class Repair
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("device_type")]
    public string? DeviceType { get; set; }

    [Column("brand")]
    public string? Brand { get; set; }

    [Column("model")]
    public string? Model { get; set; }

    [Column("problem")]
    public string? Problem { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("price")]
    public decimal? Price { get; set; }

    [Column("date_created")]
    public DateTime DateCreated { get; set; }

    [Column("date_finished")]
    public DateTime? DateFinished { get; set; }
}