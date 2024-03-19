using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.Notifications.Migrations
{
    public partial class setup_MessagingDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Messaging");

            migrationBuilder.CreateTable(
                name: "ClientApplicationVersions",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationName = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DownloadUrl = table.Column<string>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    TerminalTypeCode = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientApplicationVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevicePlatforms",
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
                    table.PrimaryKey("PK_DevicePlatforms", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    ContentAr = table.Column<string>(nullable: true),
                    ContentEn = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    TitleAr = table.Column<string>(nullable: true),
                    TitleEn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DevicePlatform = table.Column<int>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true),
                    PlatformVersion = table.Column<string>(nullable: true),
                    Vendor = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTypes",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    MessageTemplateId = table.Column<int>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTypes_MessageTemplates_MessageTemplateId",
                        column: x => x.MessageTemplateId,
                        principalSchema: "Messaging",
                        principalTable: "MessageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Messaging",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    ClientApplicationVersionId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DeviceId = table.Column<string>(nullable: true),
                    DevicePlatform = table.Column<int>(nullable: false),
                    LanguageCode = table.Column<string>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PushNotificationToken = table.Column<string>(nullable: true),
                    UserDeviceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ApplicationUserId);
                    table.ForeignKey(
                        name: "FK_Users_ClientApplicationVersions_ClientApplicationVersionId",
                        column: x => x.ClientApplicationVersionId,
                        principalSchema: "Messaging",
                        principalTable: "ClientApplicationVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_UserDevices_UserDeviceId",
                        column: x => x.UserDeviceId,
                        principalSchema: "Messaging",
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    ContentAr = table.Column<string>(nullable: true),
                    ContentEn = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MessageCategoryId = table.Column<int>(nullable: true),
                    MessageSource = table.Column<int>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    SenderId = table.Column<string>(nullable: true),
                    TitleAr = table.Column<string>(nullable: true),
                    TitleEn = table.Column<string>(nullable: true),
                    TypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_MessageCategories_MessageCategoryId",
                        column: x => x.MessageCategoryId,
                        principalSchema: "Messaging",
                        principalTable: "MessageCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_MessageTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "Messaging",
                        principalTable: "MessageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageDestinations",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    EntityType = table.Column<int>(nullable: false),
                    MessageId = table.Column<int>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageDestinations_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Messaging",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageExtraParams",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    MessageId = table.Column<int>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageExtraParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageExtraParams_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Messaging",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageStatuses",
                schema: "Messaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DeliveredDate = table.Column<DateTime>(nullable: true),
                    DevicePlatformId = table.Column<int>(nullable: true),
                    ErrorCode = table.Column<string>(nullable: true),
                    ErrorReason = table.Column<string>(nullable: true),
                    FailedDate = table.Column<DateTime>(nullable: true),
                    MessageId = table.Column<int>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ReadDate = table.Column<DateTime>(nullable: true),
                    SentDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UserApplicationUserId = table.Column<string>(nullable: true),
                    UserDeviceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageStatuses_DevicePlatforms_DevicePlatformId",
                        column: x => x.DevicePlatformId,
                        principalSchema: "Messaging",
                        principalTable: "DevicePlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageStatuses_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Messaging",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageStatuses_Users_UserApplicationUserId",
                        column: x => x.UserApplicationUserId,
                        principalSchema: "Messaging",
                        principalTable: "Users",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageStatuses_UserDevices_UserDeviceId",
                        column: x => x.UserDeviceId,
                        principalSchema: "Messaging",
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientApplicationVersions_Code",
                schema: "Messaging",
                table: "ClientApplicationVersions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DevicePlatforms_Code",
                schema: "Messaging",
                table: "DevicePlatforms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageCategories_Code",
                schema: "Messaging",
                table: "MessageCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageDestinations_Code",
                schema: "Messaging",
                table: "MessageDestinations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageDestinations_MessageId",
                schema: "Messaging",
                table: "MessageDestinations",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtraParams_Code",
                schema: "Messaging",
                table: "MessageExtraParams",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageExtraParams_MessageId",
                schema: "Messaging",
                table: "MessageExtraParams",
                column: "MessageId");

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
                name: "IX_Messages_TypeId",
                schema: "Messaging",
                table: "Messages",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_Code",
                schema: "Messaging",
                table: "MessageStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_DevicePlatformId",
                schema: "Messaging",
                table: "MessageStatuses",
                column: "DevicePlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_MessageId",
                schema: "Messaging",
                table: "MessageStatuses",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_UserApplicationUserId",
                schema: "Messaging",
                table: "MessageStatuses",
                column: "UserApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_UserDeviceId",
                schema: "Messaging",
                table: "MessageStatuses",
                column: "UserDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_Code",
                schema: "Messaging",
                table: "MessageTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_Code",
                schema: "Messaging",
                table: "MessageTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_MessageTemplateId",
                schema: "Messaging",
                table: "MessageTypes",
                column: "MessageTemplateId",
                unique: true,
                filter: "[MessageTemplateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_Code",
                schema: "Messaging",
                table: "UserDevices",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClientApplicationVersionId",
                schema: "Messaging",
                table: "Users",
                column: "ClientApplicationVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserDeviceId",
                schema: "Messaging",
                table: "Users",
                column: "UserDeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageDestinations",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "MessageExtraParams",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "MessageStatuses",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "DevicePlatforms",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "MessageCategories",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "MessageTypes",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "ClientApplicationVersions",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "UserDevices",
                schema: "Messaging");

            migrationBuilder.DropTable(
                name: "MessageTemplates",
                schema: "Messaging");
        }
    }
}
