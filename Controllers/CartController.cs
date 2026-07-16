using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagerApi.DTO;
using UserManagerApi.Interfaces;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _service.AddToCartAsync(userId, dto);

        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        return Ok(
            await _service.GetCartAsync(userId)
        );
    }
    [HttpPut("update")]
    public async Task<IActionResult> Update(
    CartAddDto dto)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _service.UpdateQuantityAsync(
            userId,
            dto.ProductId,
            dto.Quantity
        );

        return Ok();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Delete(
    int productId)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _service.RemoveAsync(
            userId,
            productId
        );

        return Ok();
    }

}