using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Media.Migrations
{
    public partial class AddStreamingUrlToVideosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreamingUrl",
                schema: "Media",
                table: "Videos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamingUrl",
                schema: "Media",
                table: "Videos");
        }
    }
}
