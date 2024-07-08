using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "PaidPosts",
                newName: "AmountPay");

            migrationBuilder.AddColumn<string>(
                name: "NomalizedTitle",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomalizedTitle",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "AmountPay",
                table: "PaidPosts",
                newName: "Rank");
        }
    }
}
