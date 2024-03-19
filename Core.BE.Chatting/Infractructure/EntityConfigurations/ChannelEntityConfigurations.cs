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
    public class ChannelEntityConfigurations : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.ToTable("Channels", ChattingDbContext.DEFAULT_SCHEMA);
            builder.HasKey(q => q.Id);
            builder.Ignore(q => q.Code);
        }
    }
}
