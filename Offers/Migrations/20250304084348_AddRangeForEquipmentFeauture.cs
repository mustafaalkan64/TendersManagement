using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.Migrations
{
    /// <inheritdoc />
    public partial class AddRangeForEquipmentFeauture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Max",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "Equipment");

            migrationBuilder.AddColumn<int>(
                name: "Max",
                table: "EquipmentFeatures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Min",
                table: "EquipmentFeatures",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Max",
                table: "EquipmentFeatures");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "EquipmentFeatures");

            migrationBuilder.AddColumn<int>(
                name: "Max",
                table: "Equipment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Min",
                table: "Equipment",
                type: "int",
                nullable: true);
        }
    }
}
