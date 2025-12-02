using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Warp.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddLinksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShorterLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TargetUrl = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: false),
                    RedirectTo = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsCustom = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPrivate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MaxClicks = table.Column<long>(type: "INTEGER", nullable: true),
                    Clicks = table.Column<long>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShorterLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShorterLinks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShorterLinks_UserId",
                table: "ShorterLinks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShorterLinks");
        }
    }
}
