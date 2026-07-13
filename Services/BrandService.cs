using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class BrandService : IBrandService
{
    private readonly ApplicationDbContext _context;

    public BrandService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        return await _context.Brands
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}