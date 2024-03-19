using Microsoft.EntityFrameworkCore;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Emeint.Core.BE.Identity.Infrastructure.Data.EnitityConfigurations
{
    public class PermittedCountryEntityConfiguration : IEntityTypeConfiguration<PermittedCountry>
    {
        public void Configure(EntityTypeBuilder<PermittedCountry> builder)
        {
            builder.HasKey(p => new { p.CountryCode, p.ApplicationUserId });
            builder.Ignore(p => p.Id);
        }
    }
}
