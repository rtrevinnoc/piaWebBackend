using Microsoft.EntityFrameworkCore;
using Monitores.Entidades;

namespace Monitores
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Branches);
        }

    }
}
