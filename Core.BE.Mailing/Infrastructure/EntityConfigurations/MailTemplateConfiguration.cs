using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Infrastructure.EntityConfigurations
{
    public class MailTemplateConfiguration : IEntityTypeConfiguration<MailTemplate>
    {
        public void Configure(EntityTypeBuilder<MailTemplate> mailDataConfiguration)
        {
            mailDataConfiguration.ToTable("MailTemplates", MailingContext.DEFAULT_SCHEMA);
            mailDataConfiguration.HasKey(m => m.Id);
            mailDataConfiguration.HasIndex(t => t.Code).IsUnique();
            mailDataConfiguration.Property<string>("MailType").IsRequired();
            mailDataConfiguration.Property<string>("TemplateNameEn").IsRequired(false);
            mailDataConfiguration.Property<string>("TemplateNameAr").IsRequired(false);
        }
    }
}