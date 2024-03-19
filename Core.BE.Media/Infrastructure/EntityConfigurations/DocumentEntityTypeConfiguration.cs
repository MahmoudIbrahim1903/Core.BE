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
    public class DocumentEntityTypeConfiguration
    : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> documentConfiguration)
        {
            documentConfiguration.ToTable("Documents", MediaContext.DEFAULT_SCHEMA);
            documentConfiguration.HasKey(i => i.Id);
        }
    }
}