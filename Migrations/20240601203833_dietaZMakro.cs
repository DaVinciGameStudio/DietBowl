using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietBowl.Migrations
{
    /// <inheritdoc />
    public partial class dietaZMakro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Calories",
                table: "Diets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Carbohydrate",
                table: "Diets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Fat",
                table: "Diets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Protein",
                table: "Diets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calories",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "Carbohydrate",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "Fat",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "Diets");
        }
    }
}
