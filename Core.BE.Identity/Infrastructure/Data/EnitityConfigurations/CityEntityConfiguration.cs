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
    public class CityEntityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("Cities");
            builder.HasKey(c => c.Code);
            builder.Ignore(c => c.Id);
            //builder.HasIndex(c => c.Code).IsUnique(true);
            builder.HasIndex(c => c.NameEn).IsUnique(true);
            builder.Property<bool>("IsActiveOperations").IsRequired(true);
            builder.Property<string>("NameEn").IsRequired(true);
        }
    }
}