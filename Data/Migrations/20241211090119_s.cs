using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class s : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_IdUserReceive",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_IdUserSend",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messengers_Users_IdUserReceive",
                table: "Messengers");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_IdUserSend",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IdUserSend",
                table: "Conversations");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdUserReceive",
                table: "Messengers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdUserReceive",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_IdUser",
                table: "Conversations",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_IdUser",
                table: "Conversations",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_IdUserReceive",
                table: "Conversations",
                column: "IdUserReceive",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messengers_Users_IdUserReceive",
                table: "Messengers",
                column: "IdUserReceive",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_IdUser",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_IdUserReceive",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messengers_Users_IdUserReceive",
                table: "Messengers");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_IdUser",
                table: "Conversations");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdUserReceive",
                table: "Messengers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdUserReceive",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdUserSend",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_IdUserSend",
                table: "Conversations",
                column: "IdUserSend");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_IdUserReceive",
                table: "Conversations",
                column: "IdUserReceive",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_IdUserSend",
                table: "Conversations",
                column: "IdUserSend",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messengers_Users_IdUserReceive",
                table: "Messengers",
                column: "IdUserReceive",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
