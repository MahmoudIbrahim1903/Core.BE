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
    public class ImageDataEntityTypeConfiguration : IEntityTypeConfiguration<ImageData>
    {
        public void Configure(EntityTypeBuilder<ImageData> imageConfiguration)
        {
            imageConfiguration.ToTable("ImagesData", MediaContext.DEFAULT_SCHEMA);
            imageConfiguration.HasKey(i => i.Id);
            imageConfiguration.Ignore(i => i.Code);
            imageConfiguration.Property(b => b.BinaryData).IsRequired();
        }
    }
}
