using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.Configurations.Migrations
{
    public partial class _Configuration19_11_2018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Configuration");

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    EnumTypeName = table.Column<string>(nullable: true),
                    Group = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Maximum = table.Column<int>(nullable: true),
                    Minimum = table.Column<int>(nullable: true),
                    ModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    SettingType = table.Column<int>(nullable: false),
                    User = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings",
                schema: "Configuration");
        }
    }
}
