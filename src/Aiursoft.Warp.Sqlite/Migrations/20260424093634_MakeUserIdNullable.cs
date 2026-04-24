using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Warp.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShorterLinks_AspNetUsers_UserId",
                table: "ShorterLinks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShorterLinks",
                type: "TEXT",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 64);

            migrationBuilder.AddForeignKey(
                name: "FK_ShorterLinks_AspNetUsers_UserId",
                table: "ShorterLinks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShorterLinks_AspNetUsers_UserId",
                table: "ShorterLinks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShorterLinks",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShorterLinks_AspNetUsers_UserId",
                table: "ShorterLinks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
