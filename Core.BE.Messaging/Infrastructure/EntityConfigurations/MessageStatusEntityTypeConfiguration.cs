using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.PlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class MessageStatusEntityTypeConfiguration : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> MessageStatusConfiguration)
        {
            MessageStatusConfiguration.ToTable("MessageStatuses", MessagingContext.DEFAULT_SCHEMA);
            MessageStatusConfiguration.HasKey(v => v.Id);

            MessageStatusConfiguration.HasIndex(v => v.Code).IsUnique();
            //MessageStatusConfiguration.Property<string>("Name").IsRequired();
            //MessageStatusConfiguration.Property<DateTime>("SentDate").IsRequired();
            //MessageStatusConfiguration.Property<DateTime>("FailedDate").IsRequired(false);
            //MessageStatusConfiguration.Property<DateTime>("DeleveredDate").IsRequired(false);
            //MessageStatusConfiguration.Property<DateTime>("ReadDate").IsRequired(false);
            //MessageStatusConfiguration.Property<DevicePlatform>("DevicePlatform").IsRequired();
            MessageStatusConfiguration.Property<string>("ErrorCode").IsRequired(false);
            MessageStatusConfiguration.Property<string>("ErrorReason").IsRequired(false);
            //MessageStatusConfiguration.Property<User>("User").IsRequired();
            //MessageStatusConfiguration.Property<UserDevice>("UserDevice").IsRequired();
        }
    }
}
