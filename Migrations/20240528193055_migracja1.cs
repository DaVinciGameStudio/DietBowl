using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietBowl.Migrations
{
    /// <inheritdoc />
    public partial class migracja1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipe_Diets_DietsId",
                table: "DietRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipe_Recipes_RecipesId",
                table: "DietRecipe");

            migrationBuilder.RenameColumn(
                name: "RecipesId",
                table: "DietRecipe",
                newName: "RecipeId");

            migrationBuilder.RenameColumn(
                name: "DietsId",
                table: "DietRecipe",
                newName: "DietId");

            migrationBuilder.RenameIndex(
                name: "IX_DietRecipe_RecipesId",
                table: "DietRecipe",
                newName: "IX_DietRecipe_RecipeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsConsumed",
                table: "DietRecipe",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipe_Diets_DietId",
                table: "DietRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_DietRecipe_Recipes_RecipeId",
                table: "DietRecipe");

            migrationBuilder.DropColumn(
                name: "IsConsumed",
                table: "DietRecipe");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "DietRecipe",
                newName: "RecipesId");

            migrationBuilder.RenameColumn(
                name: "DietId",
                table: "DietRecipe",
                newName: "DietsId");

            migrationBuilder.RenameIndex(
                name: "IX_DietRecipe_RecipeId",
                table: "DietRecipe",
                newName: "IX_DietRecipe_RecipesId");

            migrationBuilder.AddForeignKey(
                name: "FK_DietRecipe_Diets_DietsId",
                table: "DietRecipe",
                column: "DietsId",
                principalTable: "Diets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DietRecipe_Recipes_RecipesId",
                table: "DietRecipe",
                column: "RecipesId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
