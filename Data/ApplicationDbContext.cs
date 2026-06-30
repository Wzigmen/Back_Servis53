using Microsoft.EntityFrameworkCore;
using UserManagerApi.Models;
using UserManagerApi.Models;

namespace UserManagerApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
