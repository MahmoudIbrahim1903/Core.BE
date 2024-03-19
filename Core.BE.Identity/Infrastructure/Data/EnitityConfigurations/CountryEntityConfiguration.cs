using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Data.EnitityConfigurations
{
    public class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");
            builder.HasKey(k => k.Code);
            builder.Ignore(k => k.Id);

            builder.Property<string>("NameEn").IsRequired(true);
            builder.Property<string>("IsoCode").IsRequired(true);
            builder.Property<int>("PhoneNumberLength").IsRequired(true);
            builder.Property<string>("InternationalDialCode").IsRequired(true);
            builder.HasIndex(c => c.NameEn).IsUnique(true);
            //builder.HasIndex(c => c.Code).IsUnique(true);
            builder.HasIndex(c => c.IsoCode).IsUnique(true);
        }
    }
}
