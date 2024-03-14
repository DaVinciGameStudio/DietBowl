using DietBowl.Models;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.EF
{
    public class DietBowlDbContext : DbContext
    {
        public DbSet<Test> Test { get; set; }

        public DietBowlDbContext(DbContextOptions<DietBowlDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
