using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserManagerApi.Models;

[Table("phone_specs")]
public class PhoneSpec
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("screen_size")]
    public string? ScreenSize { get; set; }

    [Column("resolution")]
    public string? Resolution { get; set; }

    [Column("processor")]
    public string? Processor { get; set; }

    [Column("ram")]
    public string? Ram { get; set; }

    [Column("storage")]
    public string? Storage { get; set; }

    [Column("rear_camera")]
    public string? RearCamera { get; set; }

    [Column("front_camera")]
    public string? FrontCamera { get; set; }

    [Column("battery")]
    public string? Battery { get; set; }

    [Column("operating_system")]
    public string? OperatingSystem { get; set; }

    [Column("sim_type")]
    public string? SimType { get; set; }

    [Column("network")]
    public string? Network { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}