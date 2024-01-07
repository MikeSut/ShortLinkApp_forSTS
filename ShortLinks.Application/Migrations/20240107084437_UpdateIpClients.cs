using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShortLinks.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIpClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IpClients_Urls_UrlId",
                table: "IpClients");

            migrationBuilder.DropColumn(
                name: "ShortUrlId",
                table: "IpClients");

            migrationBuilder.AlterColumn<int>(
                name: "UrlId",
                table: "IpClients",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IpClients_Urls_UrlId",
                table: "IpClients",
                column: "UrlId",
                principalTable: "Urls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IpClients_Urls_UrlId",
                table: "IpClients");

            migrationBuilder.AlterColumn<int>(
                name: "UrlId",
                table: "IpClients",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ShortUrlId",
                table: "IpClients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_IpClients_Urls_UrlId",
                table: "IpClients",
                column: "UrlId",
                principalTable: "Urls",
                principalColumn: "Id");
        }
    }
}
