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
    public class VideoEntityTypeConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> videoConfiguration)
        {
            videoConfiguration.ToTable("Videos", MediaContext.DEFAULT_SCHEMA);
            videoConfiguration.HasKey(i => i.Id);
        }
    }
}
