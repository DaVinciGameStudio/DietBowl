using DietBowl.Models;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.EF
{
    public class DietBowlDbContext : DbContext
    {
        public virtual DbSet<Diet> Diets {get; set;}
        public virtual DbSet<Recipe> Recipes {get; set;}
        public virtual DbSet<Allergen> Allergens {get; set;}
        public virtual DbSet<BodyParameter> BodyParameters {get; set;}
        public virtual DbSet<User> Users {get; set;}
        public virtual DbSet<Preference> Preferences {get; set;}

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
