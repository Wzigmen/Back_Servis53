using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;

namespace Servis53.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public TestController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("users-count")]
    public async Task<IActionResult> GetUsersCount()
    {
        int count = await _db.Users.CountAsync();

        return Ok(new
        {
            UsersCount = count
        });
    }
}