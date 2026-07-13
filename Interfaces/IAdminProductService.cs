using UserManagerApi.DTO;

namespace UserManagerApi.Interfaces;

public interface IAdminProductService
{
    Task<int> CreateAsync(CreateProductDto dto);

    Task<bool> DeleteAsync(int id);

    Task<bool> UpdateAsync(int id, CreateProductDto dto);
}