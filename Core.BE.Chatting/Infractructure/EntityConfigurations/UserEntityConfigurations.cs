using Emeint.Core.BE.Chatting.Infractructure.Data;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Infractructure.EntityConfigurations
{
    public class UserEntityConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", ChattingDbContext.DEFAULT_SCHEMA);
            builder.HasKey(q => q.Id);
        }
    }
}
