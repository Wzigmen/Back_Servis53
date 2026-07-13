using Microsoft.EntityFrameworkCore;
using UserManagerApi.Data;
using UserManagerApi.DTO;
using UserManagerApi.Interfaces;
using UserManagerApi.Models;

namespace UserManagerApi.Services;

public class AdminProductService : IAdminProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;


    public AdminProductService(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }



    public async Task<int> CreateAsync(CreateProductDto dto)
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



        // создаём папку товара

        var productFolder = Path.Combine(

            _environment.WebRootPath,

            "images",

            "products",

            product.Id.ToString()

        );


        Directory.CreateDirectory(productFolder);



        int order = 1;


        foreach (var image in dto.Images)
        {

            if (image.Length == 0)
                continue;



            var extension =
                Path.GetExtension(image.FileName);



            var fileName =
                $"{order}{extension}";



            var filePath =
                Path.Combine(
                    productFolder,
                    fileName
                );



            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }



            var productImage = new ProductImage
            {
                ProductId = product.Id,

                ImageName = fileName,

                SortOrder = order
            };


            _context.ProductImages.Add(productImage);


            order++;
        }

        await _context.SaveChangesAsync();

        return product.Id;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var product =
            await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return false;

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        // удаляем картинки
        var folder = Path.Combine(
            _environment.WebRootPath,
            "images",
            "products",
            id.ToString()
        );

        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }

        return true;
    }

    public async Task<bool> UpdateAsync(
        int id,
        CreateProductDto dto)
    {
        var product =
            await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return false;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;

        await _context.SaveChangesAsync();

        return true;
    }
}