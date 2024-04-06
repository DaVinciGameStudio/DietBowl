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
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("DefaultConnection");
            }
        }
    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Relacja wiele do wielu dla diet i recipe
            modelBuilder.Entity<Diet>()
                .HasMany(d => d.Recipes)
                .WithMany(r => r.Diets);

            //Relacja wiele do wielu dla allergen i recipe
            modelBuilder.Entity<Allergen>()
                .HasMany(a => a.Recipes)
                .WithMany(r => r.Allergens);

            //Relacja wiele do wielu dla allergen i preference
            modelBuilder.Entity<Allergen>()
                .HasMany(a => a.Preferences)
                .WithMany(p => p.Allergens);

            //Relacja jeden do wielu dla user i bodyParameter
            modelBuilder.Entity<User>()
                .HasMany(u => u.BodyParameters)
                .WithOne(bp => bp.User)
                .HasForeignKey(bp => bp.UserId);

            //Relacja jeden do wielu dla user i diet
            modelBuilder.Entity<User>()
                .HasMany(u => u.Diets)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId);

            //Relacja jeden do jeden dla user i preferences - POPRAWIC?
            modelBuilder.Entity<User>()
                .HasOne(u => u.Preference)
                .WithOne(p => p.User)
                .HasForeignKey<Preference>(p => p.UserId);

            //Relacja jeden do jeden dla user i userNutritionalRequirement
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserNutritionalRequirement)
                .WithOne(p => p.User)
                .HasForeignKey<UserNutritionalRequirement>(p => p.UserId);

        }
    }
}
