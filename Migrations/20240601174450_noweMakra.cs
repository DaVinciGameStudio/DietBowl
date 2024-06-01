using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietBowl.Migrations
{
    /// <inheritdoc />
    public partial class noweMakra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNutritionalRequirement_Users_UserId",
                table: "UserNutritionalRequirement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNutritionalRequirement",
                table: "UserNutritionalRequirement");

            migrationBuilder.RenameTable(
                name: "UserNutritionalRequirement",
                newName: "UserNutritionalRequirements");

            migrationBuilder.RenameIndex(
                name: "IX_UserNutritionalRequirement_UserId",
                table: "UserNutritionalRequirements",
                newName: "IX_UserNutritionalRequirements_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNutritionalRequirements",
                table: "UserNutritionalRequirements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNutritionalRequirements_Users_UserId",
                table: "UserNutritionalRequirements",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNutritionalRequirements_Users_UserId",
                table: "UserNutritionalRequirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNutritionalRequirements",
                table: "UserNutritionalRequirements");

            migrationBuilder.RenameTable(
                name: "UserNutritionalRequirements",
                newName: "UserNutritionalRequirement");

            migrationBuilder.RenameIndex(
                name: "IX_UserNutritionalRequirements_UserId",
                table: "UserNutritionalRequirement",
                newName: "IX_UserNutritionalRequirement_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNutritionalRequirement",
                table: "UserNutritionalRequirement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNutritionalRequirement_Users_UserId",
                table: "UserNutritionalRequirement",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
