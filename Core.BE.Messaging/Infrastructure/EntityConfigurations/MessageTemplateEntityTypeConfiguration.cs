using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class MessageTemplateEntityTypeConfiguration : IEntityTypeConfiguration<MessageTemplate>
    {
        public void Configure(EntityTypeBuilder<MessageTemplate> MessageTemplateConfiguration)
        {
            MessageTemplateConfiguration.ToTable("MessageTemplates", MessagingContext.DEFAULT_SCHEMA);
            MessageTemplateConfiguration.HasKey(v => v.Id);

            MessageTemplateConfiguration.HasIndex(v => v.Code).IsUnique();
            MessageTemplateConfiguration.Property<string>("Name").IsRequired();
            MessageTemplateConfiguration.Property<string>("ContentEn").IsRequired();
            MessageTemplateConfiguration.Property<string>("ContentAr").IsRequired(false);
            MessageTemplateConfiguration.Property<string>("TitleEn").IsRequired(false);
            MessageTemplateConfiguration.Property<string>("TitleAr").IsRequired(false);
        }
    }
}
