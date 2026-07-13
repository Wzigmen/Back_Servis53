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
    public async Task<IActionResult> UploadImages(
    int id,
    List<IFormFile> files)
    {
        await _service.UploadImagesAsync(id, files);

        return Ok();
    }
}