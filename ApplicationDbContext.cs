using Microsoft.EntityFrameworkCore;
using Tienda.Entidades;

namespace Tienda
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<BoughtProduct> BoughtProducts { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<BoughtProduct>()
                .HasOne(c => c.User)
                .WithMany(c => c.Cart);
        }

    }
}
