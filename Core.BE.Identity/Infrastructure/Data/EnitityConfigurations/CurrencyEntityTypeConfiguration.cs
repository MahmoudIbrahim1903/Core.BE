using Emeint.Core.BE.Identity.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Data.EnitityConfigurations
{
    public class CurrencyEntityTypeConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currencies");

            //builder.HasData(new Currency
            //{
            //    Code = "EGY",
            //    CurrencyNameEn = "L.E.",
            //    CurrencyNameAr = "ج.م.",
            //    CreationDate = DateTime.UtcNow,
            //    CreatedBy = "System",
            //    Id = 1,
            //    CurrencyCode = "EGP"
            //});
        }
    }
}
