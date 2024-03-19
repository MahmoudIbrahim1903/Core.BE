﻿using Emeint.Core.BE.SMS.Infractructure.Data;
using Emeint.Core.BE.SMS.Infractructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.EntityConfigurations
{
    public class MessageStatusEntityTypeConfiguration : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.ToTable("MessageStatuses", SmsDbContext.DEFAULT_SCHEMA);
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.Code).IsUnique();
        }
    }
}
