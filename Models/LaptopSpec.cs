using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserManagerApi.Models;

[Table("laptop_specs")]
public class LaptopSpec
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

    [Column("screen_size")]
    public string? ScreenSize { get; set; }

    [Column("resolution")]
    public string? Resolution { get; set; }

    [Column("refresh_rate")]
    public string? RefreshRate { get; set; }

    [Column("operating_system")]
    public string? OperatingSystem { get; set; }

    [Column("weight")]
    public string? Weight { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}