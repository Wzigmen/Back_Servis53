using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    private readonly IWebHostEnvironment _environment;

    public ProductService(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        return await _context.Products

            .Include(x => x.Category)

            .Include(x => x.Brand)

            .Include(x => x.Images)

            .Select(x => new ProductDto
            {
                Id = x.Id,

                Name = x.Name,

                Price = x.Price,

                Category = x.Category!.Name,

                Brand = x.Brand!.Name,

                Images = x.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.ImageName)
                    .ToList()

            })

            .ToListAsync();
    }

    public async Task<ProductDetailDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products

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

        object? specs = null;

        if (product.PhoneSpec != null)
            specs = product.PhoneSpec;

        else if (product.LaptopSpec != null)
            specs = product.LaptopSpec;

        else if (product.PcSpec != null)
            specs = product.PcSpec;

        else if (product.HeadphoneSpec != null)
            specs = product.HeadphoneSpec;

        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,

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
    public async Task<object> GetProductsAsync(ProductFilterDto filter)
    {
        var query = _context.Products

            .Include(x => x.Category)

            .Include(x => x.Brand)

            .Include(x => x.Images)

            .AsQueryable();



        // поиск

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x =>
                x.Name.ToLower()
                .Contains(filter.Search.ToLower()));
        }



        // категория

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == filter.CategoryId);
        }



        // бренд

        if (filter.BrandId.HasValue)
        {
            query = query.Where(x =>
                x.BrandId == filter.BrandId);
        }



        // цена

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(x =>
                x.Price >= filter.MinPrice);
        }


        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(x =>
                x.Price <= filter.MaxPrice);
        }




        // сортировка

        query = filter.Sort switch
        {
            "priceAsc" =>
                query.OrderBy(x => x.Price),


            "priceDesc" =>
                query.OrderByDescending(x => x.Price),


            "name" =>
                query.OrderBy(x => x.Name),


            _ =>
                query.OrderByDescending(x => x.CreatedAt)
        };



        var total = await query.CountAsync();



        var products = await query

            .Skip(
                (filter.Page - 1)
                *
                filter.PageSize
            )

            .Take(filter.PageSize)



            .Select(x => new ProductDto
            {
                Id = x.Id,

                Name = x.Name,

                Price = x.Price,


                Category = x.Category!.Name,

                Brand = x.Brand!.Name,


                Images = x.Images

                    .OrderBy(i => i.SortOrder)

                    .Select(i => i.ImageName)

                    .ToList()
            })

            .ToListAsync();
        


        return new
        {
            total,

            page = filter.Page,

            pageSize = filter.PageSize,

            products
        };
    }
    public async Task<int> CreateAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            CategoryId = dto.CategoryId,

            BrandId = dto.BrandId,

            Name = dto.Name,

            Description = dto.Description,

            Price = dto.Price,

            Quantity = dto.Quantity,

            WarrantyMonths = dto.WarrantyMonths,

            IsActive = true,

            IsFeatured = false,

            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return product.Id;
    }
    public async Task UploadImagesAsync(int productId, List<IFormFile> files)
    {
        var folder = Path.Combine(
            _environment.WebRootPath,
            "images",
            "products",
            productId.ToString()
        );

        Directory.CreateDirectory(folder);

        int sort = 0;

        foreach (var file in files)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);

            _context.ProductImages.Add(new ProductImage
            {
                ProductId = productId,
                ImageName = fileName,
                SortOrder = sort++
            });
        }

        await _context.SaveChangesAsync();
    }
}