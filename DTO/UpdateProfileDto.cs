using System.ComponentModel.DataAnnotations;

namespace UserManagerApi.DTO;

public class UpdateProfileDto
{
    [StringLength(100)]
    public string? FullName { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }
}
