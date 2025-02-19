using Microsoft.EntityFrameworkCore.Migrations;

public partial class AddEquipmentModels : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create EquipmentModels table
        migrationBuilder.CreateTable(
            name: "EquipmentModels",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                EquipmentId = table.Column<int>(nullable: false),
                Model = table.Column<string>(maxLength: 100, nullable: false),
                Brand = table.Column<string>(maxLength: 100, nullable: false)
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

        // Remove Brand column from Equipment table
        migrationBuilder.DropColumn(
            name: "Brand",
            table: "Equipment");

        // Update OfferItems table
        migrationBuilder.DropColumn(
            name: "EquipmentId",
            table: "OfferItems");

        migrationBuilder.AddColumn<int>(
            name: "EquipmentModelId",
            table: "OfferItems",
            nullable: false);

        migrationBuilder.CreateIndex(
            name: "IX_OfferItems_EquipmentModelId",
            table: "OfferItems",
            column: "EquipmentModelId");

        migrationBuilder.AddForeignKey(
            name: "FK_OfferItems_EquipmentModels_EquipmentModelId",
            table: "OfferItems",
            column: "EquipmentModelId",
            principalTable: "EquipmentModels",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // ... rollback code ...
    }
} 