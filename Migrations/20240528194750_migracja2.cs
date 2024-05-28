using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietBowl.Migrations
{
    /// <inheritdoc />
    public partial class migracja2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipe_Diets_DietId",
                table: "DietRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipe_Recipes_RecipeId",
                table: "DietRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DietRecipe",
                table: "DietRecipe");

            migrationBuilder.RenameTable(
                name: "DietRecipe",
                newName: "DietRecipes");

            migrationBuilder.RenameIndex(
                name: "IX_DietRecipe_RecipeId",
                table: "DietRecipes",
                newName: "IX_DietRecipes_RecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DietRecipes",
                table: "DietRecipes",
                columns: new[] { "DietId", "RecipeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DietRecipes_Diets_DietId",
                table: "DietRecipes",
                column: "DietId",
                principalTable: "Diets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DietRecipes_Recipes_RecipeId",
                table: "DietRecipes",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipes_Diets_DietId",
                table: "DietRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipes_Recipes_RecipeId",
                table: "DietRecipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DietRecipes",
                table: "DietRecipes");

            migrationBuilder.RenameTable(
                name: "DietRecipes",
                newName: "DietRecipe");

            migrationBuilder.RenameIndex(
                name: "IX_DietRecipes_RecipeId",
                table: "DietRecipe",
                newName: "IX_DietRecipe_RecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DietRecipe",
                table: "DietRecipe",
                columns: new[] { "DietId", "RecipeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DietRecipe_Diets_DietId",
                table: "DietRecipe",
                column: "DietId",
                principalTable: "Diets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DietRecipe_Recipes_RecipeId",
                table: "DietRecipe",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
