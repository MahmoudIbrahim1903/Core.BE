using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Infrastructure.EntityConfigurations
{
    public class AudioEntityTypeConfiguration : IEntityTypeConfiguration<Audio>
    {
        public void Configure(EntityTypeBuilder<Audio> builder)
        {
            builder.ToTable("Audios", MediaContext.DEFAULT_SCHEMA);
            builder.HasKey(i => i.Id);
        }
    }
}