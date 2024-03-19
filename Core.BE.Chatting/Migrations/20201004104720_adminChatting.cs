using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Chatting.Migrations
{
    public partial class adminChatting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "Chatting",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "Chatting",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "Chatting",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Chatting",
                table: "Users");
        }
    }
}
