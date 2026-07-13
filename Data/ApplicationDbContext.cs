using Microsoft.EntityFrameworkCore;
using UserManagerApi.Models;

namespace UserManagerApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // User -> Role

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);



        // Product -> Category

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);



        // Product -> Brand

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);



        // Product -> Images

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);



        // Product -> PhoneSpec

        modelBuilder.Entity<Product>()
            .HasOne(p => p.PhoneSpec)
            .WithOne(s => s.Product)
            .HasForeignKey<PhoneSpec>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);



        // Product -> LaptopSpec

        modelBuilder.Entity<Product>()
            .HasOne(p => p.LaptopSpec)
            .WithOne(s => s.Product)
            .HasForeignKey<LaptopSpec>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);



        // Product -> PcSpec

        modelBuilder.Entity<Product>()
            .HasOne(p => p.PcSpec)
            .WithOne(s => s.Product)
            .HasForeignKey<PcSpec>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);



        // Product -> HeadphoneSpec

        modelBuilder.Entity<Product>()
            .HasOne(p => p.HeadphoneSpec)
            .WithOne(s => s.Product)
            .HasForeignKey<HeadphoneSpec>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Repair> Repairs { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<RepairHistory> RepairHistory { get; set; }
    public DbSet<AdminLog> AdminLogs { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<PhoneSpec> PhoneSpecs { get; set; }
    public DbSet<LaptopSpec> LaptopSpecs { get; set; }
    public DbSet<PcSpec> PcSpecs { get; set; }
    public DbSet<HeadphoneSpec> HeadphoneSpecs { get; set; }
}
