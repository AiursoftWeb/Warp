using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Warp.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddHitsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarpHits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LinkId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IP = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Device = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    HitTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarpHits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarpHits_ShorterLinks_LinkId",
                        column: x => x.LinkId,
                        principalTable: "ShorterLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarpHits_LinkId",
                table: "WarpHits",
                column: "LinkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarpHits");
        }
    }
}
