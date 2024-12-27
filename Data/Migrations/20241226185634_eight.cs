using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class eight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAdvertisementPricing_Services_IdService",
                table: "ServiceAdvertisementPricing");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdService",
                table: "ServiceAdvertisementPricing",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ServiceAdvertisementPricing",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "ServiceAdvertisementPricing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAdvertisementPricing_Services_IdService",
                table: "ServiceAdvertisementPricing",
                column: "IdService",
                principalTable: "Services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAdvertisementPricing_Services_IdService",
                table: "ServiceAdvertisementPricing");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "ServiceAdvertisementPricing");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdService",
                table: "ServiceAdvertisementPricing",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ServiceAdvertisementPricing",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAdvertisementPricing_Services_IdService",
                table: "ServiceAdvertisementPricing",
                column: "IdService",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
