﻿// <auto-generated />
using System;
using DietBowl.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DietBowl.Migrations
{
    [DbContext(typeof(DietBowlDbContext))]
    partial class DietBowlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AllergenPreference", b =>
                {
                    b.Property<int>("AllergensId")
                        .HasColumnType("int");

                    b.Property<int>("PreferencesId")
                        .HasColumnType("int");

                    b.HasKey("AllergensId", "PreferencesId");

                    b.HasIndex("PreferencesId");

                    b.ToTable("AllergenPreference");
                });

            modelBuilder.Entity("AllergenRecipe", b =>
                {
                    b.Property<int>("AllergensId")
                        .HasColumnType("int");

                    b.Property<int>("RecipesId")
                        .HasColumnType("int");

                    b.HasKey("AllergensId", "RecipesId");

                    b.HasIndex("RecipesId");

                    b.ToTable("AllergenRecipe");
                });

            modelBuilder.Entity("DietBowl.Models.Allergen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Allergens");
                });

            modelBuilder.Entity("DietBowl.Models.BodyParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("BMI")
                        .HasColumnType("float");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double>("Height")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("BodyParameters");
                });

            modelBuilder.Entity("DietBowl.Models.Diet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Diets");
                });

            modelBuilder.Entity("DietBowl.Models.DietRecipe", b =>
                {
                    b.Property<int>("DietId")
                        .HasColumnType("int");

                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsConsumed")
                        .HasColumnType("bit");

                    b.HasKey("DietId", "RecipeId");

                    b.HasIndex("RecipeId");

                    b.ToTable("DietRecipes");
                });

            modelBuilder.Entity("DietBowl.Models.Preference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ActivityStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WeightGoal")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Preferences");
                });

            modelBuilder.Entity("DietBowl.Models.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Calories")
                        .HasColumnType("float");

                    b.Property<double>("Carbohydrate")
                        .HasColumnType("float");

                    b.Property<double>("Fat")
                        .HasColumnType("float");

                    b.Property<string>("Ingedients")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Instructions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Protein")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("DietBowl.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IdDietician")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Sex")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DietBowl.Models.UserNutritionalRequirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Calories")
                        .HasColumnType("float");

                    b.Property<double>("Carbohydrate")
                        .HasColumnType("float");

                    b.Property<double>("Fat")
                        .HasColumnType("float");

                    b.Property<double>("Protein")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserNutritionalRequirement");
                });

            modelBuilder.Entity("AllergenPreference", b =>
                {
                    b.HasOne("DietBowl.Models.Allergen", null)
                        .WithMany()
                        .HasForeignKey("AllergensId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DietBowl.Models.Preference", null)
                        .WithMany()
                        .HasForeignKey("PreferencesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AllergenRecipe", b =>
                {
                    b.HasOne("DietBowl.Models.Allergen", null)
                        .WithMany()
                        .HasForeignKey("AllergensId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DietBowl.Models.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DietBowl.Models.BodyParameter", b =>
                {
                    b.HasOne("DietBowl.Models.User", "User")
                        .WithMany("BodyParameters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietBowl.Models.Diet", b =>
                {
                    b.HasOne("DietBowl.Models.User", "User")
                        .WithMany("Diets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietBowl.Models.DietRecipe", b =>
                {
                    b.HasOne("DietBowl.Models.Diet", "Diet")
                        .WithMany("DietRecipes")
                        .HasForeignKey("DietId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DietBowl.Models.Recipe", "Recipe")
                        .WithMany("DietRecipes")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Diet");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("DietBowl.Models.Preference", b =>
                {
                    b.HasOne("DietBowl.Models.User", "User")
                        .WithOne("Preference")
                        .HasForeignKey("DietBowl.Models.Preference", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietBowl.Models.UserNutritionalRequirement", b =>
                {
                    b.HasOne("DietBowl.Models.User", "User")
                        .WithOne("UserNutritionalRequirement")
                        .HasForeignKey("DietBowl.Models.UserNutritionalRequirement", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietBowl.Models.Diet", b =>
                {
                    b.Navigation("DietRecipes");
                });

            modelBuilder.Entity("DietBowl.Models.Recipe", b =>
                {
                    b.Navigation("DietRecipes");
                });

            modelBuilder.Entity("DietBowl.Models.User", b =>
                {
                    b.Navigation("BodyParameters");

                    b.Navigation("Diets");

                    b.Navigation("Preference")
                        .IsRequired();

                    b.Navigation("UserNutritionalRequirement")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
