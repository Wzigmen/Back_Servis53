using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagerApi.Models;

[Table("repairhistory")]
public class RepairHistory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("repair_id")]
    public int RepairId { get; set; }

    [Column("status")]
    public string Status { get; set; } = "";

    [Column("changed_at")]
    public DateTime ChangedAt { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }
}
