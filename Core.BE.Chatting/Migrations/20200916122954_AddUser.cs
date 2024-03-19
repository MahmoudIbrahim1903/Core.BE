using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Chatting.Migrations
{
    public partial class AddUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberId",
                schema: "Chatting",
                table: "ChannelMembers");

            migrationBuilder.DropColumn(
                name: "MemberName",
                schema: "Chatting",
                table: "ChannelMembers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Chatting",
                table: "ChannelMembers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Chatting",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembers_UserId",
                schema: "Chatting",
                table: "ChannelMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_Users_UserId",
                schema: "Chatting",
                table: "ChannelMembers",
                column: "UserId",
                principalSchema: "Chatting",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMembers_Users_UserId",
                schema: "Chatting",
                table: "ChannelMembers");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Chatting");

            migrationBuilder.DropIndex(
                name: "IX_ChannelMembers_UserId",
                schema: "Chatting",
                table: "ChannelMembers");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Chatting",
                table: "ChannelMembers");

            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                schema: "Chatting",
                table: "ChannelMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberName",
                schema: "Chatting",
                table: "ChannelMembers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
