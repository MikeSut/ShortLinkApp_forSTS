using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShortLinks.Application.Migrations
{
    /// <inheritdoc />
    public partial class updateUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TgName",
                table: "TgChatIdUsers");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "TgChatIdUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "TgChatIdUsers");

            migrationBuilder.AddColumn<string>(
                name: "TgName",
                table: "TgChatIdUsers",
                type: "text",
                nullable: true);
        }
    }
}
