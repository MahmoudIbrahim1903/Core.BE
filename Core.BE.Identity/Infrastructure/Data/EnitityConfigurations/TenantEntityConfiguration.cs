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
//    public class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
//    {
//        public void Configure(EntityTypeBuilder<Tenant> builder)
//        {
//            builder.ToTable("Tenants");
//            builder.HasKey(t => t.Id);
//            //builder.HasData(new Tenant()
//            //{
//            //    Code = "TN-1",
//            //    CreatedBy = "System",
//            //    CreationDate = DateTime.UtcNow,
//            //    NameEn = "TN-1",
//            //    NameAr = "TN-1",
//            //    Id = 1
//            //});
//        }
//    }
//}