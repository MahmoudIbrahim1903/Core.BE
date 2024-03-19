using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.SMS.Migrations
{
    public partial class FirstsmsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Sms");

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                schema: "Sms",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentEn = table.Column<string>(nullable: true),
                    ContentAr = table.Column<string>(nullable: true),
                    TemplateName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Sms",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsMessages",
                schema: "Sms",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MessageText = table.Column<string>(nullable: true),
                    SmsProviderType = table.Column<int>(nullable: false),
                    MessageTemplateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsMessages_MessageTemplates_MessageTemplateId",
                        column: x => x.MessageTemplateId,
                        principalSchema: "Sms",
                        principalTable: "MessageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageUsers",
                schema: "Sms",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    SmsMessageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageUsers_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalSchema: "Sms",
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageUsers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Sms",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageStatuses",
                schema: "Sms",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StatusCode = table.Column<int>(nullable: false),
                    StatusMessage = table.Column<string>(nullable: true),
                    MessageUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageStatuses_MessageUsers_MessageUserId",
                        column: x => x.MessageUserId,
                        principalSchema: "Sms",
                        principalTable: "MessageUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_Code",
                schema: "Sms",
                table: "MessageStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_MessageUserId",
                schema: "Sms",
                table: "MessageStatuses",
                column: "MessageUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_Code",
                schema: "Sms",
                table: "MessageTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsers_Code",
                schema: "Sms",
                table: "MessageUsers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsers_SmsMessageId",
                schema: "Sms",
                table: "MessageUsers",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsers_UserId",
                schema: "Sms",
                table: "MessageUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_Code",
                schema: "Sms",
                table: "SmsMessages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_MessageTemplateId",
                schema: "Sms",
                table: "SmsMessages",
                column: "MessageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Code",
                schema: "Sms",
                table: "Users",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageStatuses",
                schema: "Sms");

            migrationBuilder.DropTable(
                name: "MessageUsers",
                schema: "Sms");

            migrationBuilder.DropTable(
                name: "SmsMessages",
                schema: "Sms");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Sms");

            migrationBuilder.DropTable(
                name: "MessageTemplates",
                schema: "Sms");
        }
    }
}
