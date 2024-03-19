using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.Mailing.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Mailing");

            migrationBuilder.CreateTable(
                name: "Mails",
                schema: "Mailing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(nullable: false),
                    Cc = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    From = table.Column<string>(nullable: false),
                    FromUserId = table.Column<string>(nullable: true),
                    HasAttachments = table.Column<bool>(nullable: true),
                    IsImportant = table.Column<bool>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: false),
                    To = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailTemplates",
                schema: "Mailing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    MailType = table.Column<string>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TemplateNameAr = table.Column<string>(nullable: false),
                    TemplateNameEn = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mails_Code",
                schema: "Mailing",
                table: "Mails",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_Code",
                schema: "Mailing",
                table: "MailTemplates",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mails",
                schema: "Mailing");

            migrationBuilder.DropTable(
                name: "MailTemplates",
                schema: "Mailing");
        }
    }
}
