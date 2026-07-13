using UserManagerApi.DTO;

namespace UserManagerApi.Interfaces;

public interface IProductService
{
    Task<object> GetProductsAsync(ProductFilterDto filter);
    Task<ProductDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(ProductCreateDto dto);
    Task UploadImagesAsync(int productId, List<IFormFile> files);
}