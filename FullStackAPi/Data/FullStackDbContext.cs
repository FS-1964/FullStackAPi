using FullStackAPi.Models;
using Microsoft.EntityFrameworkCore;

namespace FullStackAPi.Data
{
    public class FullStackDbContext : DbContext
    {
        public FullStackDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShoppingCard> Shoppingcard { get; set; }

    }
}
