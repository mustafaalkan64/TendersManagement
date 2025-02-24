using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.Migrations
{
    /// <inheritdoc />
    public partial class AddTeklifSunumTarihi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TeklifSunumTarihi",
                table: "Offers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeklifSunumTarihi",
                table: "Offers");
        }
    }
}
