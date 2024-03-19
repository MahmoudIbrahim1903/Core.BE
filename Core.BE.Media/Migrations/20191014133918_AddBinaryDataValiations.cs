using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Media.Migrations
{
    public partial class AddBinaryDataValiations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "BinaryData",
                schema: "Media",
                table: "ImagesData",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "BinaryData",
                schema: "Media",
                table: "ImagesData",
                nullable: true,
                oldClrType: typeof(byte[]));
        }
    }
}
