using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserManagerApi.Models;

[Table("headphone_specs")]
public class HeadphoneSpec
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("headphone_type")]
    public string? HeadphoneType { get; set; }

    [Column("wireless")]
    public bool? Wireless { get; set; }

    [Column("bluetooth_version")]
    public string? BluetoothVersion { get; set; }

    [Column("noise_canceling")]
    public bool? NoiseCanceling { get; set; }

    [Column("battery_life")]
    public string? BatteryLife { get; set; }

    [Column("microphone")]
    public bool? Microphone { get; set; }

    [Column("weight")]
    public string? Weight { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}