using System.ComponentModel.DataAnnotations;

namespace UserManagerApi.DTO;

public class RepairCreateDto
{
    [StringLength(100)]
    public string? ClientName { get; set; }

    [StringLength(30)]
    public string? ClientPhone { get; set; }

    [EmailAddress, StringLength(100)]
    public string? ClientEmail { get; set; }

    [StringLength(100)]
    public string? DeviceType { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [StringLength(2000)]
    public string? Problem { get; set; }
}
