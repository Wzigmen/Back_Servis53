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
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId);
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
}
