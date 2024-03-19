using Emeint.Core.BE.Notifications.Domain.AggregatesModel.ApplicationAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> UserConfiguration)
        {
            UserConfiguration.ToTable("Users", MessagingContext.DEFAULT_SCHEMA);
            UserConfiguration.Ignore(n => n.Id);
            UserConfiguration.HasKey(n => n.ApplicationUserId);

            //UserConfiguration.HasIndex(v => v.Code).IsUnique();
            UserConfiguration.Property<string>("Name").IsRequired(false);
            UserConfiguration.Property<string>("LanguageCode").IsRequired(false);
            UserConfiguration.Property<string>("DeviceId").IsRequired(false);
            //UserConfiguration.Property<string>("PushNotificationToken").IsRequired(false);
        }
    }
}