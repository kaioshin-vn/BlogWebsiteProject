using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class six : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaidPosts");

            migrationBuilder.DropTable(
                name: "PaymentRequests");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PostViews");

            migrationBuilder.CreateTable(
                name: "PostHideByRestricted",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPost = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdRestricted = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostHideByRestricted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostHideByRestricted_Posts_IdPost",
                        column: x => x.IdPost,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PostHideByRestricted_RestrictedWords_IdRestricted",
                        column: x => x.IdRestricted,
                        principalTable: "RestrictedWords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAdvertisementPricing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdService = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAdvertisementPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAdvertisementPricing_Services_IdService",
                        column: x => x.IdService,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationAdvertisements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdServiceAdvertis = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentAds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    TimeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeAccept = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationAdvertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationAdvertisements_ServiceAdvertisementPricing_IdServiceAdvertis",
                        column: x => x.IdServiceAdvertis,
                        principalTable: "ServiceAdvertisementPricing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RegistrationAdvertisements_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdRegis = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_RegistrationAdvertisements_IdRegis",
                        column: x => x.IdRegis,
                        principalTable: "RegistrationAdvertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_IdRegis",
                table: "Invoices",
                column: "IdRegis");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_IdUser",
                table: "Invoices",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_PostHideByRestricted_IdPost",
                table: "PostHideByRestricted",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_PostHideByRestricted_IdRestricted",
                table: "PostHideByRestricted",
                column: "IdRestricted");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationAdvertisements_IdServiceAdvertis",
                table: "RegistrationAdvertisements",
                column: "IdServiceAdvertis");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationAdvertisements_IdUser",
                table: "RegistrationAdvertisements",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAdvertisementPricing_IdService",
                table: "ServiceAdvertisementPricing",
                column: "IdService");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PostHideByRestricted");

            migrationBuilder.DropTable(
                name: "RegistrationAdvertisements");

            migrationBuilder.DropTable(
                name: "ServiceAdvertisementPricing");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.CreateTable(
                name: "PaidPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPost = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountPay = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaidPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaidPosts_Posts_IdPost",
                        column: x => x.IdPost,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaidPosts_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VnPayResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PostViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostViews_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostViews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaidPosts_IdPost",
                table: "PaidPosts",
                column: "IdPost");

            migrationBuilder.CreateIndex(
                name: "IX_PaidPosts_IdUser",
                table: "PaidPosts",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_IdUser",
                table: "PaymentRequests",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_IdUser",
                table: "PaymentTransactions",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_PostId",
                table: "PostViews",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_UserId",
                table: "PostViews",
                column: "UserId");
        }
    }
}
