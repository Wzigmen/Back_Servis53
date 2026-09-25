using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Helpers;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ImageStorage _images;

    public ProductService(ApplicationDbContext context, ImageStorage images)
    {
        _context = context;
        _images = images;
    }

    public async Task<PagedResultDto<ProductDto>> GetProductsAsync(ProductFilterDto filter)
    {
        var page = Math.Max(filter.Page, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, ProductFilterDto.MaxPageSize);

        var query = _context.Products.AsNoTracking();

        // поиск
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search));
        }

        // категория
        if (filter.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == filter.CategoryId);

        // бренд
        if (filter.BrandId.HasValue)
            query = query.Where(x => x.BrandId == filter.BrandId);

        // цена
        if (filter.MinPrice.HasValue)
            query = query.Where(x => x.Price >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(x => x.Price <= filter.MaxPrice);

        // сортировка
        query = filter.Sort switch
        {
            "priceAsc" => query.OrderBy(x => x.Price),
            "priceDesc" => query.OrderByDescending(x => x.Price),
            "name" => query.OrderBy(x => x.Name),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        var total = await query.CountAsync();

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity,
                Category = x.Category!.Name,
                Brand = x.Brand!.Name,
                Images = x.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.ImageName)
                    .ToList()
            })
            .ToListAsync();

        return new PagedResultDto<ProductDto>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Products = products
        };
    }

    public async Task<ProductDetailDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .Include(x => x.Images)
            .Include(x => x.PhoneSpec)
            .Include(x => x.LaptopSpec)
            .Include(x => x.PcSpec)
            .Include(x => x.HeadphoneSpec)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return null;

        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            WarrantyMonths = product.WarrantyMonths,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            Brand = product.Brand?.Name,
            Category = product.Category?.Name,
            Images = product.Images
                .OrderBy(x => x.SortOrder)
                .Select(x => x.ImageName)
                .ToList(),
            Phone = product.PhoneSpec,
            Laptop = product.LaptopSpec,
            Pc = product.PcSpec,
            Headphones = product.HeadphoneSpec
        };
    }

    public Task<bool> ExistsAsync(int id) =>
        _context.Products.AnyAsync(x => x.Id == id);

    public async Task<string?> ValidateAsync(ProductCreateDto dto)
    {
        if (!await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
            return "Категория не найдена.";

        if (!await _context.Brands.AnyAsync(x => x.Id == dto.BrandId))
            return "Бренд не найден.";

        return null;
    }

    public async Task<int> CreateAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            IsActive = true,
            IsFeatured = false,
            CreatedAt = DateTime.UtcNow
        };

        Apply(product, dto);

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return product.Id;
    }

    public async Task<bool> UpdateAsync(int id, ProductCreateDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return false;

        Apply(product, dto);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task UploadImagesAsync(int productId, IEnumerable<IFormFile> files)
    {
        var folder = _images.ProductFolder(productId);

        // новые фото добавляются после уже существующих
        var sort = await _context.ProductImages
            .Where(x => x.ProductId == productId)
            .Select(x => (int?)x.SortOrder)
            .MaxAsync() ?? -1;

        foreach (var file in files)
        {
            var fileName = await _images.SaveAsync(file, folder, Guid.NewGuid().ToString());

            _context.ProductImages.Add(new ProductImage
            {
                ProductId = productId,
                ImageName = fileName,
                SortOrder = ++sort
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task SavePhoneSpecAsync(int productId, PhoneSpecCreateDto dto)
    {
        var spec = await _context.PhoneSpecs.FirstOrDefaultAsync(x => x.ProductId == productId);

        if (spec == null)
        {
            spec = new PhoneSpec { ProductId = productId };
            _context.PhoneSpecs.Add(spec);
        }

        spec.ScreenSize = dto.ScreenSize;
        spec.Resolution = dto.Resolution;
        spec.Processor = dto.Processor;
        spec.Ram = dto.Ram;
        spec.Storage = dto.Storage;
        spec.RearCamera = dto.RearCamera;
        spec.FrontCamera = dto.FrontCamera;
        spec.Battery = dto.Battery;
        spec.OperatingSystem = dto.OperatingSystem;
        spec.SimType = dto.SimType;
        spec.Network = dto.Network;

        await _context.SaveChangesAsync();
    }

    public async Task<DeleteProductResult> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return DeleteProductResult.NotFound;

        // товар из оформленных заказов удалять нельзя — сломается история заказов
        if (await _context.OrderItems.AnyAsync(x => x.ProductId == id))
            return DeleteProductResult.UsedInOrders;

        // у избранного и отзывов в БД нет каскадного удаления
        _context.Favorites.RemoveRange(_context.Favorites.Where(x => x.ProductId == id));
        _context.Reviews.RemoveRange(_context.Reviews.Where(x => x.ProductId == id));

        // фото, характеристики и позиции корзин удалятся каскадно
        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        _images.DeleteFolder(_images.ProductFolder(id));

        return DeleteProductResult.Deleted;
    }

    private static void Apply(Product product, ProductCreateDto dto)
    {
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.Name = dto.Name.Trim();
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.WarrantyMonths = dto.WarrantyMonths;
    }
}
