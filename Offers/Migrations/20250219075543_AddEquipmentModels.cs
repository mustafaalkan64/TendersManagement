using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferItems_Equipment_EquipmentId",
                table: "OfferItems");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Equipment");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "OfferItems",
                newName: "EquipmentModelId");

            migrationBuilder.RenameIndex(
                name: "IX_OfferItems_EquipmentId",
                table: "OfferItems",
                newName: "IX_OfferItems_EquipmentModelId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Equipment",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Equipment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentModels_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentModels_EquipmentId",
                table: "EquipmentModels",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OfferItems_EquipmentModels_EquipmentModelId",
                table: "OfferItems",
                column: "EquipmentModelId",
                principalTable: "EquipmentModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferItems_EquipmentModels_EquipmentModelId",
                table: "OfferItems");

            migrationBuilder.DropTable(
                name: "EquipmentModels");

            migrationBuilder.RenameColumn(
                name: "EquipmentModelId",
                table: "OfferItems",
                newName: "EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_OfferItems_EquipmentModelId",
                table: "OfferItems",
                newName: "IX_OfferItems_EquipmentId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Equipment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Equipment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Equipment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OfferItems_Equipment_EquipmentId",
                table: "OfferItems",
                column: "EquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
