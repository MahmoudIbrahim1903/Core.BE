using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Infrastructure.EntityConfigurations
{
    public class SettingEntityTypeConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Settings", ConfigurationContext.DEFAULT_SCHEMA);
            builder.HasKey(i => i.Id);
            builder.Ignore(i => i.id);
            builder.Property(i => i.IsRequired)
                .IsRequired()
                .HasDefaultValue(true);
            
        }
    }
}
