using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Notifications.Migrations
{
    public partial class messaging_missed_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageTypes_MessageTemplateId",
                schema: "Messaging",
                table: "MessageTypes");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_MessageTemplateId",
                schema: "Messaging",
                table: "MessageTypes",
                column: "MessageTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageTypes_MessageTemplateId",
                schema: "Messaging",
                table: "MessageTypes");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_MessageTemplateId",
                schema: "Messaging",
                table: "MessageTypes",
                column: "MessageTemplateId",
                unique: true,
                filter: "[MessageTemplateId] IS NOT NULL");
        }
    }
}
