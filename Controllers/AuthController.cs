using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
using UserManagerApi.Interfaces;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!await _authService.RegisterAsync(dto))
            return BadRequest("Пользователь уже существует.");

        return Ok("Регистрация успешна.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
            return Unauthorized("Неверный логин или пароль.");

        return Ok(new
        {
            token = result.Token,
            user = result.User
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = User.GetUserId();

        if (id == null)
            return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(id.Value);

        return user == null ? Unauthorized() : Ok(user);
    }
}
