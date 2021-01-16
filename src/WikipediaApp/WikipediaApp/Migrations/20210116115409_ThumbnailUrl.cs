using Microsoft.EntityFrameworkCore.Migrations;

namespace WikipediaApp.Migrations
{
    public partial class ThumbnailUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "History",
                newName: "url");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Favorites",
                newName: "url");

            migrationBuilder.AddColumn<string>(
                name: "thumbnailUrl",
                table: "History",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thumbnailUrl",
                table: "Favorites",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnailUrl",
                table: "History");

            migrationBuilder.DropColumn(
                name: "thumbnailUrl",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "History",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "Favorites",
                newName: "Url");
        }
    }
}
