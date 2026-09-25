using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
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

    // [Authorize] гарантирует наличие токена, а значит и id
    private int UserId => User.GetUserId()!.Value;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetCartAsync(UserId));
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        var error = await _service.AddToCartAsync(UserId, dto);

        return error == null ? Ok() : BadRequest(error);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(AddToCartDto dto)
    {
        var error = await _service.UpdateQuantityAsync(UserId, dto.ProductId, dto.Quantity);

        return error == null ? Ok() : BadRequest(error);
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Delete(int productId)
    {
        await _service.RemoveAsync(UserId, productId);

        return Ok();
    }
}
