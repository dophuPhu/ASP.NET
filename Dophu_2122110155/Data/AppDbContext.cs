using Microsoft.EntityFrameworkCore;
using Dophu_2122110155.Model;

namespace Dophu_2122110155.Data
{
    
    public class AppDbContext : DbContext
    {
     
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

       
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Category { get; set; }

    }
}
