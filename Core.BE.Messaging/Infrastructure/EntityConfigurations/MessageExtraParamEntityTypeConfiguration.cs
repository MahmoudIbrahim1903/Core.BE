using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
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
    public class MessageExtraParamEntityTypeConfiguration : IEntityTypeConfiguration<MessageExtraParam>
    {
        public void Configure(EntityTypeBuilder<MessageExtraParam> MessageExtraParamConfiguration)
        {
            MessageExtraParamConfiguration.ToTable("MessageExtraParams", MessagingContext.DEFAULT_SCHEMA);
            MessageExtraParamConfiguration.HasKey(v => v.Id);

            MessageExtraParamConfiguration.HasIndex(v => v.Code).IsUnique();
        }
    }
}
