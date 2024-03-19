using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.Mailing.Migrations
{
    public partial class updataMailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorCode",
                schema: "Mailing",
                table: "Mails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                schema: "Mailing",
                table: "Mails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Mailing",
                table: "Mails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorCode",
                schema: "Mailing",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                schema: "Mailing",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Mailing",
                table: "Mails");
        }
    }
}
