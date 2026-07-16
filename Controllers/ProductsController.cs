using Microsoft.AspNetCore.Mvc;
using UserManagerApi.DTO;
using UserManagerApi.Interfaces;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ProductFilterDto filter)
    {
        return Ok(await _service.GetProductsAsync(filter));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var id = await _service.CreateAsync(dto);

        return Ok(new
        {
            id
        });
    }
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadImages(int id, [FromForm] List<IFormFile> files)
    {
        await _service.UploadImagesAsync(id, files);

        return Ok();
    }

    // Создание спецификации продукта
    [HttpPost("{id}/phone")]
    public async Task<IActionResult> CreatePhoneSpec(
    int id,
    PhoneSpecCreateDto dto)
    {
        await _service.CreatePhoneSpecAsync(id, dto);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}