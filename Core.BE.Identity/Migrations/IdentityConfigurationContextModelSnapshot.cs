﻿// <auto-generated />
using System;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Emeint.Core.BE.Identity.Migrations
{
    [DbContext(typeof(IdentityConfigurationContext))]
    partial class IdentityConfigurationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.BE.Configurations.Domain.Model.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
