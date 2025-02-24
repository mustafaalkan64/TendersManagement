using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Offers.Migrations
{
    /// <inheritdoc />
    public partial class TeklifTarihleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DanismanlikSonTeklifBitis",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DanismanlikSonTeklifSunum",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DanismanlikTeklifGecerlilikSuresi",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DanismanlikTeklifGonderim",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProjectAddress",
                table: "Offers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SonTeklifBildirme",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TeklifGecerlilikSuresi",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TeklifGonderimTarihi",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DanismanlikSonTeklifBitis",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "DanismanlikSonTeklifSunum",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "DanismanlikTeklifGecerlilikSuresi",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "DanismanlikTeklifGonderim",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ProjectAddress",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "SonTeklifBildirme",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "TeklifGecerlilikSuresi",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "TeklifGonderimTarihi",
                table: "Offers");
        }
    }
}
