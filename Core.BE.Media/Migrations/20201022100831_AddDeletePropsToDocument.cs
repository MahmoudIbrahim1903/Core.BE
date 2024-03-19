using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Media.Migrations
{
    public partial class AddDeletePropsToDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Media",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                schema: "Media",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Media",
                table: "Documents",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Media",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                schema: "Media",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Media",
                table: "Documents");
        }
    }
}
