using Emeint.Core.BE.Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emeint.Core.BE.Infrastructure.EntityConfigurations
{
    public class ClientCommandEntityTypeConfiguration
       : IEntityTypeConfiguration<ClientCommand>
    {
        public void Configure(EntityTypeBuilder<ClientCommand> requestConfiguration)
        {
            requestConfiguration.ToTable("Commands", "Logging");
            requestConfiguration.HasKey(cr => cr.RequestClientReferenceNumber);
            requestConfiguration.Property(cr => cr.Name).IsRequired();
            requestConfiguration.Property(cr => cr.Command).IsRequired();
            requestConfiguration.Property(cr => cr.Time).IsRequired();
        }
    }
}
