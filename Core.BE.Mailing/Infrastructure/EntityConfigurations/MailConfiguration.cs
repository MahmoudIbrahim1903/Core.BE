using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Infrastructure.EntityConfigurations
{
    public class MailConfiguration : IEntityTypeConfiguration<Mail>
    {
        public void Configure(EntityTypeBuilder<Mail> mailDataConfiguration)
        {
            mailDataConfiguration.ToTable("Mails", MailingContext.DEFAULT_SCHEMA);
            mailDataConfiguration.HasKey(m => m.Id);
            mailDataConfiguration.HasIndex(t => t.Code).IsUnique();
            mailDataConfiguration.Property<string>("From").IsRequired();
            mailDataConfiguration.Property<string>("To").IsRequired();
            mailDataConfiguration.Property<string>("Cc").IsRequired(false);
            mailDataConfiguration.Property<string>("Subject").IsRequired();
            mailDataConfiguration.Property<string>("Body").IsRequired();
            mailDataConfiguration.Property<bool?>("HasAttachments").IsRequired(false);
            mailDataConfiguration.Property<bool?>("IsImportant").IsRequired(false);
            mailDataConfiguration.Property<string>("Type").IsRequired(false);
            mailDataConfiguration.Property<string>("FromUserId").IsRequired(false);
        }
    }
}
