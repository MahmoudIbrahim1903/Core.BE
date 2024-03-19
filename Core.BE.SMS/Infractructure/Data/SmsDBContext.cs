using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Infrastructure.DBContext;
using Emeint.Core.BE.SMS.Infractructure.EntityConfigurations;
using Emeint.Core.BE.SMS.Infractructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.SMS.Infractructure.Data
{
    public class SmsDbContext : CountryBaseDBContext
    {
        public SmsDbContext(DbContextOptions<DbContext> options, IConfiguration configuration, IIdentityService identityService) 
            : base(options, configuration, identityService) { }
        public SmsDbContext()
        {
            
        }

        public static string DEFAULT_SCHEMA = "Sms";
        public DbSet<SmsMessage> SmsMessages { set; get; }
        public DbSet<MessageTemplate> MessageTemplates { set; get; }
        public DbSet<User> Users { set; get; }
        public DbSet<MessageStatus> MessageStatuses { set; get; }
        public DbSet<MessageUser> MessageUsers { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new MessageTemplateEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SmsMessageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageUserEntityTypeConfiguration());
        }

        public virtual void Seed()
        {

        }
    }
}
