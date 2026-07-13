using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserManagerApi.Models;

[Table("pc_specs")]
public class PcSpec
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("cpu")]
    public string? Cpu { get; set; }

    [Column("gpu")]
    public string? Gpu { get; set; }

    [Column("ram")]
    public string? Ram { get; set; }

    [Column("storage")]
    public string? Storage { get; set; }

    [Column("motherboard")]
    public string? Motherboard { get; set; }

    [Column("power_supply")]
    public string? PowerSupply { get; set; }

    [Column("case_name")]
    public string? CaseName { get; set; }

    [Column("cooling")]
    public string? Cooling { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}