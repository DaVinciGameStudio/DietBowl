using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DietBowl.Models;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.EF
{
    public class UserDbContext : DbContext
    {
         public DbSet<User> Users { get; set; }

         public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // Nazwa tabeli w bazie danych
                entity.HasKey(e => e.Id); // Ustawienie klucza głównego
                // Mapowanie właściwości do kolumn w bazie danych
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.isDietician).IsRequired();
            });
        }
    }
}