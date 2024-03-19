using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.Configurations.Migrations
{
    public partial class release40Conf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Configuration",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "Configuration",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInPortal",
                schema: "Configuration",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Configuration",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "Configuration",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ShowInPortal",
                schema: "Configuration",
                table: "Settings");
        }
    }
}
