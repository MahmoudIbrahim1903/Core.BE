﻿// <auto-generated />
using Emeint.Core.BE.Configurations.Domain.Enums;
using Emeint.Core.BE.Configurations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Emeint.Core.BE.Configurations.Migrations
{
    [DbContext(typeof(ConfigurationContext))]
    partial class ConfigurationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.BE.Configurations.Domain.Model.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName");

                    b.Property<string>("EnumTypeName");

                    b.Property<string>("Group");

                    b.Property<string>("Key");

                    b.Property<int?>("Maximum");

                    b.Property<int?>("Minimum");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<int>("SettingType");

                    b.Property<bool?>("ShowInPortal");

                    b.Property<string>("UnitOfMeasure");

                    b.Property<int>("User");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings","Configuration");
                });
#pragma warning restore 612, 618
        }
    }
}
