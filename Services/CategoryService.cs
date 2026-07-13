using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}