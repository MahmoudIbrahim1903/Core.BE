using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class MessageTypeEntityTypeConfiguration : IEntityTypeConfiguration<MessageType>
    {
        public void Configure(EntityTypeBuilder<MessageType> messagetypeConfiguration)
        {
            messagetypeConfiguration.ToTable("MessageTypes", MessagingContext.DEFAULT_SCHEMA);
            messagetypeConfiguration.HasKey(v => v.Id);

            messagetypeConfiguration.HasIndex(v => v.Code).IsUnique();
            messagetypeConfiguration.Property<string>("Name").IsRequired();
        }
    }
}
