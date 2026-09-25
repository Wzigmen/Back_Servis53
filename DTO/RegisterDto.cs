using System.ComponentModel.DataAnnotations;

namespace UserManagerApi.DTO;

public class RegisterDto
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Login { get; set; } = "";

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = "";

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }
}
