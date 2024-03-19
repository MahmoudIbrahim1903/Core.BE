using Emeint.Core.BE.API.Infrastructure.Middlewares.LoggingMiddlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emeint.Core.BE.Infrastructure.EntityConfigurations
{
    public class ClientRequestLoggingMiddlewareEntityTypeConfiguration
       : IEntityTypeConfiguration<ClientRequestLoggingMiddleware>
    {
        public void Configure(EntityTypeBuilder<ClientRequestLoggingMiddleware> requestConfiguration)
        {
            requestConfiguration.ToTable("Requests", "Logging");
            requestConfiguration.HasKey(cr => cr.Id);
            requestConfiguration.Property(cr => cr.RequestClientReferenceNumber);
            requestConfiguration.Property(cr => cr.HttpMethod).IsRequired();
            requestConfiguration.Property(cr => cr.Path).IsRequired();
            requestConfiguration.Property(cr => cr.Headers).IsRequired();
            requestConfiguration.Property(cr => cr.Body).IsRequired();
            requestConfiguration.Property(cr => cr.Time).IsRequired();
        }
    }
}