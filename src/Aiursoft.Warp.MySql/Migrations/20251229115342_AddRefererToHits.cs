using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Warp.MySql.Migrations
{
    /// <inheritdoc />
    public partial class AddRefererToHits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Referer",
                table: "WarpHits",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Referer",
                table: "WarpHits");
        }
    }
}
