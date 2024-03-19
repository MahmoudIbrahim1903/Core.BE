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
//    public class CountryRoleEntityConfigurations : IEntityTypeConfiguration<CountryRole>
//    {
//        public void Configure(EntityTypeBuilder<CountryRole> builder)
//        {
//            builder.ToTable("CountryRoles");
//            //builder.HasData(
//            //    new CountryRole
//            //    {
//            //        Id = 1,
//            //        Code = "EGY-SuperAdmin",
//            //        RoleName = "SuperAdmin",
//            //        CreatedBy = "System",
//            //        CreationDate = DateTime.UtcNow,
//            //        CountryCode = "EGY"
//            //    });
//        }
//    }
//}
