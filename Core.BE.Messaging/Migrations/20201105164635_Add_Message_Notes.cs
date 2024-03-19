using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Notifications.Migrations
{
    public partial class Add_Message_Notes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "Messaging",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "Messaging",
                table: "Messages");
        }
    }
}
