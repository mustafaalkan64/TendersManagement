using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectOwnerId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ProjectOwnerId",
                table: "Offers",
                column: "ProjectOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_ProjectOwners_ProjectOwnerId",
                table: "Offers",
                column: "ProjectOwnerId",
                principalTable: "ProjectOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_ProjectOwners_ProjectOwnerId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_ProjectOwnerId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ProjectOwnerId",
                table: "Offers");
        }
    }
}
