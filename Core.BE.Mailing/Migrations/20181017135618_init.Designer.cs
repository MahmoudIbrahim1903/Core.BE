﻿// <auto-generated />
using Emeint.Core.BE.Mailing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Emeint.Core.BE.Mailing.Migrations
{
    [DbContext(typeof(MailingContext))]
    [Migration("20181017135618_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.BE.Mailing.Domain.AggregatesModel.Mail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .IsRequired();

                    b.Property<string>("Cc")
                        .IsRequired();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("From")
                        .IsRequired();

                    b.Property<string>("FromUserId");

                    b.Property<bool?>("HasAttachments");

                    b.Property<bool?>("IsImportant");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.Property<string>("To")
                        .IsRequired();

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Mails","Mailing");
                });

            modelBuilder.Entity("Core.BE.Mailing.Domain.AggregatesModel.MailTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("MailType")
                        .IsRequired();

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("TemplateNameAr")
                        .IsRequired();

                    b.Property<string>("TemplateNameEn")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("MailTemplates","Mailing");
                });
#pragma warning restore 612, 618
        }
    }
}
