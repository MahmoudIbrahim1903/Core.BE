//using Emeint.Core.BE.Identity.Domain.Model;
//using Emeint.Core.BE.Identity.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Emeint.Core.BE.Identity.Infrastructure.Data.EnitityConfigurations
//{
//    public class TenantCountryEntityConfiguration : IEntityTypeConfiguration<TenantCountry>
//    {
//        public void Configure(EntityTypeBuilder<TenantCountry> builder)
//        {
//            builder.ToTable("TenantCountries");
//            builder.HasKey(tc => new { tc.TenantId, tc.CountryId });
//            builder.Ignore(tc => tc.Id);
//            builder.Ignore(tc => tc.Code);
//        }
//    }
//}
