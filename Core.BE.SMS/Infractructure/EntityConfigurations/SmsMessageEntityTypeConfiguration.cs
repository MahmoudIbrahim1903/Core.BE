using Emeint.Core.BE.SMS.Infractructure.Data;
using Emeint.Core.BE.SMS.Infractructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Emeint.Core.BE.SMS.Infractructure.EntityConfigurations
{
    public class SmsMessageEntityTypeConfiguration : IEntityTypeConfiguration<SmsMessage>
    {
        public void Configure(EntityTypeBuilder<SmsMessage> builder)
        {
            builder.ToTable("SmsMessages", SmsDbContext.DEFAULT_SCHEMA);
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.Code).IsUnique();
        }
    }
}
