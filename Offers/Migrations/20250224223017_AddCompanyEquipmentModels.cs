using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyEquipmentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyEquipmentModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    EquipmentModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEquipmentModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyEquipmentModels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyEquipmentModels_EquipmentModels_EquipmentModelId",
                        column: x => x.EquipmentModelId,
                        principalTable: "EquipmentModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEquipmentModels_CompanyId",
                table: "CompanyEquipmentModels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEquipmentModels_EquipmentModelId",
                table: "CompanyEquipmentModels",
                column: "EquipmentModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyEquipmentModels");
        }
    }
}
