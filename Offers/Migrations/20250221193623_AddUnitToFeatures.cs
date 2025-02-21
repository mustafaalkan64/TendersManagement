using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitToFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "EquipmentModelFeatures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "EquipmentFeatures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentModelFeatures_UnitId",
                table: "EquipmentModelFeatures",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentFeatures_UnitId",
                table: "EquipmentFeatures",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentFeatures_Units_UnitId",
                table: "EquipmentFeatures",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentModelFeatures_Units_UnitId",
                table: "EquipmentModelFeatures",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentFeatures_Units_UnitId",
                table: "EquipmentFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentModelFeatures_Units_UnitId",
                table: "EquipmentModelFeatures");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentModelFeatures_UnitId",
                table: "EquipmentModelFeatures");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentFeatures_UnitId",
                table: "EquipmentFeatures");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "EquipmentModelFeatures");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "EquipmentFeatures");
        }
    }
}
