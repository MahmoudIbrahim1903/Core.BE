﻿// <auto-generated />
using System;
using Emeint.Core.BE.Notifications.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Emeint.Core.BE.Notifications.Migrations
{
    [DbContext(typeof(MessagingContext))]
    [Migration("20191023160313_EnhanceMessaging")]
    partial class EnhanceMessaging
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.ApplicationAggregate.ClientApplicationVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicationName");

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("DownloadUrl");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("TerminalTypeCode");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.ToTable("ClientApplicationVersions","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.DevicePlatformAggregate.DevicePlatform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.ToTable("DevicePlatforms","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("ContentAr");

                    b.Property<string>("ContentEn");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("MessageSource");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("SenderId");

                    b.Property<string>("TitleAr");

                    b.Property<string>("TitleEn");

                    b.Property<int?>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.HasIndex("TypeId");

                    b.ToTable("Messages","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageDestination", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Destination");

                    b.Property<int>("DestinationType");

                    b.Property<int?>("MessageId");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.HasIndex("MessageId");

                    b.ToTable("MessageDestinations","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageExtraParam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Key");

                    b.Property<int?>("MessageId");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.HasIndex("MessageId");

                    b.ToTable("MessageExtraParams","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("DeliveredDate");

                    b.Property<int?>("DevicePlatformId");

                    b.Property<string>("ErrorCode");

                    b.Property<string>("ErrorReason");

                    b.Property<DateTime?>("FailedDate");

                    b.Property<int?>("MessageId");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("ReadDate");

                    b.Property<DateTime?>("SentDate");

                    b.Property<int>("Status");

                    b.Property<string>("UserApplicationUserId");

                    b.Property<int?>("UserDeviceId");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.HasIndex("DevicePlatformId");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserApplicationUserId");

                    b.HasIndex("UserDeviceId");

                    b.ToTable("MessageStatuses","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("ContentAr");

                    b.Property<string>("ContentEn")
                        .IsRequired();

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("TitleAr");

                    b.Property<string>("TitleEn");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.ToTable("MessageTemplates","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<int?>("MessageTemplateId");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.HasIndex("MessageTemplateId");

                    b.ToTable("MessageTypes","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.UserAggregate.User", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientApplicationVersionId");

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("DeviceId");

                    b.Property<int>("DevicePlatform");

                    b.Property<string>("LanguageCode");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<string>("PushNotificationToken");

                    b.Property<int?>("UserDeviceId");

                    b.HasKey("ApplicationUserId");

                    b.HasIndex("ClientApplicationVersionId");

                    b.HasIndex("UserDeviceId");

                    b.ToTable("Users","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.UserAggregate.UserDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Comment");

                    b.Property<string>("CreatedBy")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("DevicePlatform");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<string>("Platform");

                    b.Property<string>("PlatformVersion");

                    b.Property<string>("Vendor");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[Code] IS NOT NULL");

                    b.ToTable("UserDevices","Messaging");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.Message", b =>
                {
                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageType", "Type")
                        .WithMany("Message")
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageDestination", b =>
                {
                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.Message", "Message")
                        .WithMany("MessageDestinations")
                        .HasForeignKey("MessageId");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageExtraParam", b =>
                {
                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.Message", "Message")
                        .WithMany("MessageExtraParams")
                        .HasForeignKey("MessageId");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageStatus", b =>
                {
                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.DevicePlatformAggregate.DevicePlatform", "DevicePlatform")
                        .WithMany()
                        .HasForeignKey("DevicePlatformId");

                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.Message", "Message")
                        .WithMany("Statuses")
                        .HasForeignKey("MessageId");

                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany("MessageStatuses")
                        .HasForeignKey("UserApplicationUserId");

                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.UserAggregate.UserDevice", "UserDevice")
                        .WithMany()
                        .HasForeignKey("UserDeviceId");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageType", b =>
                {
                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.MessageAggregate.MessageTemplate", "MessageTemplate")
                        .WithMany()
                        .HasForeignKey("MessageTemplateId");
                });

            modelBuilder.Entity("Core.BE.Messaging.Domain.AggregatesModel.UserAggregate.User", b =>
                {
                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.ApplicationAggregate.ClientApplicationVersion", "ClientApplicationVersion")
                        .WithMany()
                        .HasForeignKey("ClientApplicationVersionId");

                    b.HasOne("Core.BE.Messaging.Domain.AggregatesModel.UserAggregate.UserDevice", "UserDevice")
                        .WithMany()
                        .HasForeignKey("UserDeviceId");
                });
#pragma warning restore 612, 618
        }
    }
}
