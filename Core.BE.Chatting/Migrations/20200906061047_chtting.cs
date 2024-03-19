using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Chatting.Migrations
{
    public partial class chtting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Chatting");

            migrationBuilder.CreateTable(
                name: "Channels",
                schema: "Chatting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    FirstMemberId = table.Column<string>(nullable: true),
                    FirstMemberName = table.Column<string>(nullable: true),
                    SecondMemberId = table.Column<string>(nullable: true),
                    SecondMemberName = table.Column<string>(nullable: true),
                    ChannelUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channels",
                schema: "Chatting");
        }
    }
}
