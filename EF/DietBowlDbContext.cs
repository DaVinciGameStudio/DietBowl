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
        public virtual DbSet<PreferenceAllergen> PreferenceAllers { get; set;}
        public virtual DbSet<DietRecipe> DietRecipes { get; set;}
        public virtual DbSet<RecipeAllergen> RecipeAllers { get; set;}

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
            //modelBuilder.Entity<Allergen>()
            //    .HasOne(a => a.Recipe)
            //    .WithMany(r => r.Allergens)
            //    .HasForeignKey(a => a.RecipeId)
            //    .OnDelete(DeleteBehavior.Restrict); // Zapobieganie kaskadowemu usuwaniu

            //modelBuilder.Entity<Allergen>()
            //    .HasOne(a => a.Preference)
            //    .WithMany(p => p.Allergens)
            //    .HasForeignKey(a => a.PreferenceId);
        }
    }
}
