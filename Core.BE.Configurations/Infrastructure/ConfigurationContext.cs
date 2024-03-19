using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure.EntityConfigurations;
using Emeint.Core.BE.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Configurations.Infrastructure
{
    public class ConfigurationContext : CountryBaseDBContext
    {
        public ConfigurationContext(DbContextOptions<DbContext> options, IConfiguration configuration, IIdentityService identityService)
            : base(options, configuration, identityService) { }

        public ConfigurationContext() : base()
        {
        }

        public const string DEFAULT_SCHEMA = "Configuration";
        public DbSet<Setting> Settings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SettingEntityTypeConfiguration());
            //modelBuilder.Entity<Setting>().Property(s => s.ShowInPortal).HasDefaultValue(true);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public virtual void Seed()
        {

        }
    }
}
