using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations
{
    public class MessageDestinationEntityTypeConfiguration : IEntityTypeConfiguration<MessageDestination>
    {
        public void Configure(EntityTypeBuilder<MessageDestination> MessageDestinationConfiguration)
        {
            MessageDestinationConfiguration.ToTable("MessageDestinations", MessagingContext.DEFAULT_SCHEMA);
            MessageDestinationConfiguration.HasKey(v => v.Id);

            MessageDestinationConfiguration.HasIndex(v => v.Code).IsUnique();
            //MessageDestinationConfiguration.Property<Message>("Message").IsRequired();
            //MessageDestinationConfiguration.Property<int>("EntityType").IsRequired();
            //MessageDestinationConfiguration.Property<UserTag>("UserTag").IsRequired();
        }
    }
}
