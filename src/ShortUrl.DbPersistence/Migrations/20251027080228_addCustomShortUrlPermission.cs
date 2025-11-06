using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShortUrl.DbPersistence.Migrations
{
    /// <inheritdoc />
    public partial class addCustomShortUrlPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanSetCustomShortCodes",
                table: "ApiKeys",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanSetCustomShortCodes",
                table: "ApiKeys");
        }
    }
}
