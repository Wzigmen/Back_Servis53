using UserManagerApi.Models;

namespace UserManagerApi.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllAsync();
}