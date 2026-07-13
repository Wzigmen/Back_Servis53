using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagerApi.DTO;
using UserManagerApi.Interfaces;

namespace UserManagerApi.Controllers.Admin;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : ControllerBase
{
    private readonly IAdminProductService _service;

    public AdminProductsController(IAdminProductService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
    {
        var id = await _service.CreateAsync(dto);

        return Ok(new
        {
            id
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,
        [FromForm] CreateProductDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);

        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);

        if (!success)
            return NotFound();

        return Ok();
    }
}