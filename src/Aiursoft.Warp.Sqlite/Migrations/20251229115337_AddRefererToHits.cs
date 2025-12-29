using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Warp.Sqlite.Migrations
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
                type: "TEXT",
                maxLength: 1000,
                nullable: true);
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
