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
    public int? UserId { get; set; }

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

    [Column("client_name")]
    public string? ClientName { get; set; }

    [Column("client_phone")]
    public string? ClientPhone { get; set; }

    [Column("client_email")]
    public string? ClientEmail { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}