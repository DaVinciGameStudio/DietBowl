using DietBowl.Models;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.EF
{
    public class DietBowlDbContext : DbContext
    {
        public DbSet<Diet> Diets {get; set;}
        public DbSet<Recipe> Recipes {get; set;}
        public DbSet<Allergen> Allergens {get; set;}
        public DbSet<BodyParameter> BodyParameters {get; set;}
        public DbSet<User> Users {get; set;}
        public DbSet<Preference> Preferences {get; set;}

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
