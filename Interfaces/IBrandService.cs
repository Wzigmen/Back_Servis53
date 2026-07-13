using UserManagerApi.Models;

namespace UserManagerApi.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<Brand>> GetAllAsync();
}