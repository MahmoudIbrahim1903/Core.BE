using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emeint.Core.BE.Notifications.Domain.Enums;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> MessageConfiguration)
        {
            MessageConfiguration.ToTable("Messages", MessagingContext.DEFAULT_SCHEMA);
            MessageConfiguration.HasKey(v => v.Id);

            MessageConfiguration.HasIndex(v => v.Code).IsUnique();
            //MessageConfiguration.Property<MessageStatus>("Status").IsRequired();
            //MessageConfiguration.Property<MessageType>("Type").IsRequired();
            //MessageConfiguration.Property<string>("MessageCategory").IsRequired(false);
            MessageConfiguration.Property<string>("SenderId").IsRequired(false);
            //MessageConfiguration.Property<string>("Description").IsRequired(false);
            MessageConfiguration.Property<bool>("IsDeleted").IsRequired();
        }
    }
}
