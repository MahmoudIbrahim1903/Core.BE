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
    public class AreaEntityConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            builder.ToTable("Areas");
            builder.HasKey(a => a.Code);
            builder.Ignore(a => a.Id);
            //builder.HasIndex(a => a.Code).IsUnique(true);
            //builder.HasIndex(a => new { a.City.Code, a.NameEn }).IsUnique(true);
            //builder.HasIndex(a => new { a.City.Code, a.NameAr }).IsUnique(true);
            builder.Property<string>("NameEn").IsRequired(true);
        }
    }
}