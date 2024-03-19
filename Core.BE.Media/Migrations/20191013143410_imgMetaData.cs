using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Media.Migrations
{
    public partial class imgMetaData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images",
                schema: "Media");

            migrationBuilder.CreateTable(
                name: "ImagesData",
                schema: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    BinaryData = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImagesMetaData",
                schema: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ClientReferenceNumber = table.Column<string>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    TenantCode = table.Column<string>(nullable: true),
                    OriginalBinaryDataLength = table.Column<int>(nullable: true),
                    ImageDataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesMetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagesMetaData_ImagesData_ImageDataId",
                        column: x => x.ImageDataId,
                        principalSchema: "Media",
                        principalTable: "ImagesData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagesMetaData_ClientReferenceNumber",
                schema: "Media",
                table: "ImagesMetaData",
                column: "ClientReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagesMetaData_Code",
                schema: "Media",
                table: "ImagesMetaData",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ImagesMetaData_ImageDataId",
                schema: "Media",
                table: "ImagesMetaData",
                column: "ImageDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagesMetaData",
                schema: "Media");

            migrationBuilder.DropTable(
                name: "ImagesData",
                schema: "Media");

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
                    TenantCode = table.Column<string>(nullable: true),
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
    }
}
