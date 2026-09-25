using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
using UserManagerApi.Interfaces;

namespace UserManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] ProductFilterDto filter)
    {
        return Ok(await _service.GetProductsAsync(filter));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);

        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var error = await _service.ValidateAsync(dto);

        if (error != null)
            return BadRequest(error);

        var id = await _service.CreateAsync(dto);

        return Ok(new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductCreateDto dto)
    {
        var error = await _service.ValidateAsync(dto);

        if (error != null)
            return BadRequest(error);

        return await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/images")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<IActionResult> UploadImages(int id, [FromForm] List<IFormFile> files)
    {
        if (!await _service.ExistsAsync(id))
            return NotFound();

        if (files.Count == 0)
            return BadRequest("Файлы не выбраны.");

        foreach (var file in files)
        {
            var error = ImageStorage.Validate(file);

            if (error != null)
                return BadRequest($"{file.FileName}: {error}");
        }

        await _service.UploadImagesAsync(id, files);

        return Ok();
    }

    // Создание или обновление характеристик смартфона
    [HttpPost("{id:int}/phone")]
    public async Task<IActionResult> SavePhoneSpec(int id, PhoneSpecCreateDto dto)
    {
        if (!await _service.ExistsAsync(id))
            return NotFound();

        await _service.SavePhoneSpecAsync(id, dto);

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _service.DeleteAsync(id) switch
        {
            DeleteProductResult.NotFound => NotFound(),
            DeleteProductResult.UsedInOrders => Conflict("Товар есть в заказах, удалить его нельзя. Поставьте количество 0."),
            _ => NoContent()
        };
    }
}
