namespace UserManagerApi.DTO;

public class RegisterDto
{
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string FullName { get; set; } = "";
}