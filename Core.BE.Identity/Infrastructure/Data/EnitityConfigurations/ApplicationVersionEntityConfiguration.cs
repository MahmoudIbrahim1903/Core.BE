using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emeint.Core.BE.Identity.Infrastructure.Data.EnitityConfigurations
{
    public class ApplicationVersionEntityConfiguration : IEntityTypeConfiguration<ApplicationVersion>
    {
        public void Configure(EntityTypeBuilder<ApplicationVersion> builder)
        {
            builder.ToTable("ApplicationVersions");
            builder.HasKey(k => k.Id);
            builder.HasIndex(a => a.Code).IsUnique(true);
        }
    }
}
