using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.Media.Migrations
{
    public partial class ImageTableIntial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Media");

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BinaryData = table.Column<byte[]>(nullable: false),
                    ClientReferenceNumber = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Height = table.Column<int>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OriginalBinaryDataLength = table.Column<int>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ClientReferenceNumber",
                schema: "Media",
                table: "Images",
                column: "ClientReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_Code",
                schema: "Media",
                table: "Images",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images",
                schema: "Media");
        }
    }
}
