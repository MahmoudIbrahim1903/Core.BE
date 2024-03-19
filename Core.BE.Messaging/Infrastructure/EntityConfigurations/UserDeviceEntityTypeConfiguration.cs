using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
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
    public class UserDeviceEntityTypeConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> UserDeviceConfiguration)
        {
            UserDeviceConfiguration.ToTable("UserDevices", MessagingContext.DEFAULT_SCHEMA);
            UserDeviceConfiguration.HasKey(v => v.Id);

            UserDeviceConfiguration.HasIndex(v => v.Code).IsUnique();
            UserDeviceConfiguration.Property<string>("Name").IsRequired(false);
            //UserDeviceConfiguration.Property<User>("User").IsRequired(false);
            //UserDeviceConfiguration.Property<DevicePlatform>("DevicePlatform").IsRequired(false);
            UserDeviceConfiguration.Property<string>("PlatformVersion").IsRequired(false);
            UserDeviceConfiguration.Property<string>("Vendor").IsRequired(false);
            UserDeviceConfiguration.Property<string>("Platform").IsRequired(false);
        }
    }
}
