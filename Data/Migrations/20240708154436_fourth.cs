using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdPost",
                table: "Responses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Responses_IdPost",
                table: "Responses",
                column: "IdPost");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Posts_IdPost",
                table: "Responses",
                column: "IdPost",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Posts_IdPost",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Responses_IdPost",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "IdPost",
                table: "Responses");
        }
    }
}
