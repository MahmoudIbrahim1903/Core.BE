using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Chatting.Migrations
{
    public partial class chatmembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstMemberId",
                schema: "Chatting",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "FirstMemberName",
                schema: "Chatting",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "SecondMemberId",
                schema: "Chatting",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "SecondMemberName",
                schema: "Chatting",
                table: "Channels");

            migrationBuilder.CreateTable(
                name: "ChannelMembers",
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
                    ChannelId = table.Column<int>(nullable: false),
                    MemberId = table.Column<string>(nullable: true),
                    MemberName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelMembers_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalSchema: "Chatting",
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembers_ChannelId",
                schema: "Chatting",
                table: "ChannelMembers",
                column: "ChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelMembers",
                schema: "Chatting");

            migrationBuilder.AddColumn<string>(
                name: "FirstMemberId",
                schema: "Chatting",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstMemberName",
                schema: "Chatting",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondMemberId",
                schema: "Chatting",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondMemberName",
                schema: "Chatting",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
