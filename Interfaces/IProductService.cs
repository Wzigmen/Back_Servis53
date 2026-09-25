using UserManagerApi.DTO;

namespace UserManagerApi.Interfaces;

public enum DeleteProductResult
{
    Deleted,
    NotFound,
    UsedInOrders
}

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetProductsAsync(ProductFilterDto filter);
    Task<ProductDetailDto?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);

    // Возвращает текст ошибки, если категория или бренд не существуют
    Task<string?> ValidateAsync(ProductCreateDto dto);

    Task<int> CreateAsync(ProductCreateDto dto);
    Task<bool> UpdateAsync(int id, ProductCreateDto dto);
    Task UploadImagesAsync(int productId, IEnumerable<IFormFile> files);
    Task SavePhoneSpecAsync(int productId, PhoneSpecCreateDto dto);
    Task<DeleteProductResult> DeleteAsync(int id);
}
