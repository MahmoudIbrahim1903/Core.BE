using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emeint.Core.BE.Notifications.Migrations
{
    public partial class EnhanceMessaging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MessageCategories_MessageCategoryId",
                schema: "Messaging",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "MessageCategories",
                schema: "Messaging");

            migrationBuilder.DropIndex(
                name: "IX_UserDevices_Code",
                schema: "Messaging",
                table: "UserDevices");

            migrationBuilder.DropIndex(
                name: "IX_MessageTypes_Code",
                schema: "Messaging",
                table: "MessageTypes");

            migrationBuilder.DropIndex(
                name: "IX_MessageTemplates_Code",
                schema: "Messaging",
                table: "MessageTemplates");

            migrationBuilder.DropIndex(
                name: "IX_MessageStatuses_Code",
                schema: "Messaging",
                table: "MessageStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Code",
                schema: "Messaging",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessageCategoryId",
                schema: "Messaging",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_MessageExtraParams_Code",
                schema: "Messaging",
                table: "MessageExtraParams");

            migrationBuilder.DropIndex(
                name: "IX_MessageDestinations_Code",
                schema: "Messaging",
                table: "MessageDestinations");

            migrationBuilder.DropIndex(
                name: "IX_DevicePlatforms_Code",
                schema: "Messaging",
                table: "DevicePlatforms");

            migrationBuilder.DropIndex(
                name: "IX_ClientApplicationVersions_Code",
                schema: "Messaging",
                table: "ClientApplicationVersions");

            migrationBuilder.DropColumn(
                name: "MessageCategoryId",
                schema: "Messaging",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EntityType",
                schema: "Messaging",
                table: "MessageDestinations");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "UserDevices",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageTypes",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageTemplates",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageStatuses",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageExtraParams",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageDestinations",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                schema: "Messaging",
                table: "MessageDestinations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationType",
                schema: "Messaging",
                table: "MessageDestinations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "DevicePlatforms",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "ClientApplicationVersions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_Code",
                schema: "Messaging",
                table: "UserDevices",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_Code",
                schema: "Messaging",
                table: "MessageTypes",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_Code",
                schema: "Messaging",
                table: "MessageTemplates",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_Code",
                schema: "Messaging",
                table: "MessageStatuses",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Code",
                schema: "Messaging",
                table: "Messages",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtraParams_Code",
                schema: "Messaging",
                table: "MessageExtraParams",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MessageDestinations_Code",
                schema: "Messaging",
                table: "MessageDestinations",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DevicePlatforms_Code",
                schema: "Messaging",
                table: "DevicePlatforms",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClientApplicationVersions_Code",
                schema: "Messaging",
                table: "ClientApplicationVersions",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDevices_Code",
                schema: "Messaging",
                table: "UserDevices");

            migrationBuilder.DropIndex(
                name: "IX_MessageTypes_Code",
                schema: "Messaging",
                table: "MessageTypes");

            migrationBuilder.DropIndex(
                name: "IX_MessageTemplates_Code",
                schema: "Messaging",
                table: "MessageTemplates");

            migrationBuilder.DropIndex(
                name: "IX_MessageStatuses_Code",
                schema: "Messaging",
                table: "MessageStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Code",
                schema: "Messaging",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_MessageExtraParams_Code",
                schema: "Messaging",
                table: "MessageExtraParams");

            migrationBuilder.DropIndex(
                name: "IX_MessageDestinations_Code",
                schema: "Messaging",
                table: "MessageDestinations");

            migrationBuilder.DropIndex(
                name: "IX_DevicePlatforms_Code",
                schema: "Messaging",
                table: "DevicePlatforms");

            migrationBuilder.DropIndex(
                name: "IX_ClientApplicationVersions_Code",
                schema: "Messaging",
                table: "ClientApplicationVersions");

            migrationBuilder.DropColumn(
                name: "Destination",
                schema: "Messaging",
                table: "MessageDestinations");

            migrationBuilder.DropColumn(
                name: "DestinationType",
                schema: "Messaging",
                table: "MessageDestinations");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "UserDevices",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageTypes",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageTemplates",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageStatuses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageCategoryId",
                schema: "Messaging",
                table: "Messages",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageExtraParams",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "MessageDestinations",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntityType",
                schema: "Messaging",
                table: "MessageDestinations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "DevicePlatforms",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Messaging",
                table: "ClientApplicationVersions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MessageCategories",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_Code",
                schema: "Messaging",
                table: "UserDevices",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_Code",
                schema: "Messaging",
                table: "MessageTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_Code",
                schema: "Messaging",
                table: "MessageTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_Code",
                schema: "Messaging",
                table: "MessageStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Code",
                schema: "Messaging",
                table: "Messages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageCategoryId",
                schema: "Messaging",
                table: "Messages",
                column: "MessageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtraParams_Code",
                schema: "Messaging",
                table: "MessageExtraParams",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageDestinations_Code",
                schema: "Messaging",
                table: "MessageDestinations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DevicePlatforms_Code",
                schema: "Messaging",
                table: "DevicePlatforms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientApplicationVersions_Code",
                schema: "Messaging",
                table: "ClientApplicationVersions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageCategories_Code",
                schema: "Messaging",
                table: "MessageCategories",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MessageCategories_MessageCategoryId",
                schema: "Messaging",
                table: "Messages",
                column: "MessageCategoryId",
                principalSchema: "Messaging",
                principalTable: "MessageCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
