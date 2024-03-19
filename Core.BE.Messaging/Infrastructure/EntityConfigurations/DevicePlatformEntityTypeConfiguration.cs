using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.PlatformAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class DevicePlatformEntityTypeConfiguration : IEntityTypeConfiguration<DevicePlatform>
    {
        public void Configure(EntityTypeBuilder<DevicePlatform> devicePlatformEntityTypeConfiguration)
        {
            devicePlatformEntityTypeConfiguration.ToTable("DevicePlatforms", MessagingContext.DEFAULT_SCHEMA);
            devicePlatformEntityTypeConfiguration.HasKey(v => v.Id);
            
            devicePlatformEntityTypeConfiguration.HasIndex(v => v.Code).IsUnique();
            devicePlatformEntityTypeConfiguration.Property<string>("Name").IsRequired();
        }
    }
}
