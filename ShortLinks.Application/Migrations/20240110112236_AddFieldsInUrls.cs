using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShortLinks.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsInUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LifeTimeLink",
                table: "Urls",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Permanent",
                table: "Urls",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LifeTimeLink",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "Permanent",
                table: "Urls");
        }
    }
}
