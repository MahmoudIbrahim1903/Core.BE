using Emeint.Core.BE.Notifications.Domain.AggregatesModel.ApplicationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class ClientApplicationVersionEntityTypeConfiguration : IEntityTypeConfiguration<ClientApplicationVersion>
    {
        public void Configure(EntityTypeBuilder<ClientApplicationVersion> ClientApplicationVersionConfiguration)
        {
            ClientApplicationVersionConfiguration.ToTable("ClientApplicationVersions", MessagingContext.DEFAULT_SCHEMA);
            ClientApplicationVersionConfiguration.HasKey(v => v.Id);

            ClientApplicationVersionConfiguration.HasIndex(v => v.Code).IsUnique();
            ClientApplicationVersionConfiguration.Property<string>("Name").IsRequired();
            ClientApplicationVersionConfiguration.Property<string>("TerminalTypeCode").IsRequired(false);
            ClientApplicationVersionConfiguration.Property<string>("Description").IsRequired(false);
            ClientApplicationVersionConfiguration.Property<string>("ApplicationName").IsRequired(false);
            ClientApplicationVersionConfiguration.Property<string>("DownloadUrl").IsRequired(false);
            ClientApplicationVersionConfiguration.Property<string>("Version").IsRequired(false);
        }
    }
}
