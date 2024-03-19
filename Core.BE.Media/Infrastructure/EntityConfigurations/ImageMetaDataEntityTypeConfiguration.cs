using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Emeint.Core.BE.Media.Infrastructure.EntityConfigurations
{
    public class ImageMetaDataEntityTypeConfiguration
       : IEntityTypeConfiguration<ImageMetaData>
    {
        public void Configure(EntityTypeBuilder<ImageMetaData> imageConfiguration)
        {
            imageConfiguration.ToTable("ImagesMetaData", MediaContext.DEFAULT_SCHEMA);
            imageConfiguration.HasKey(i => i.Id);
            imageConfiguration.Ignore(i => i.id);
            imageConfiguration.HasIndex(i => i.Code).IsUnique();
            imageConfiguration.HasIndex(i => i.ClientReferenceNumber).IsUnique();
            imageConfiguration.Property(i => i.ClientReferenceNumber).IsRequired();
            imageConfiguration.Property(i => i.ImagePath);
            imageConfiguration.Property(i => i.Width);
            imageConfiguration.Property(i => i.Height);
            imageConfiguration.Property(i => i.Name);
            imageConfiguration.Property(i => i.Tag);
            imageConfiguration.Property(i => i.TenantCode);
            imageConfiguration.Property(i => i.OriginalBinaryDataLength);
            //imageConfiguration.Property(i => i.BinaryData).IsRequired();
            //imageConfiguration.Property(i => i.ScalingMode).HasMaxLength(1);
        }
    }
}
