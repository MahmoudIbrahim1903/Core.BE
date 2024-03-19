using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data.EnitityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Emeint.Core.BE.Identity.Infrastructure.Data
{
    public class CountryContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        //  public const string DEFAULT_SCHEMA = "Identity";
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        //public DbSet<Tenant> Tenants { get; set; }
        //public DbSet<TenantCountry> TenantsCountries { get; set; }
        //public DbSet<CountryRole> CountryRoles { set; get; }


        public CountryContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public CountryContext(DbContextOptions<CountryContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CountryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CurrencyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CityEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AreaEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TenantCountryEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new CountryRoleEntityConfigurations());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                var connectionString =
                    _configuration["ConnectionString"];

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public virtual void Seed()
        {
            if (!Countries.Any(c => c.Code == "EGY"))
            {
                Countries.Add(new Country
                {
                    Code = "EGY",
                    DefaultLanguage = "ar",
                    Id = 1,
                    CreatedBy = "System",
                    NameEn = "Egypt",
                    NameAr = "مصر",
                    AllowMultipleRoles = true,
                    InternationalDialCode = "20",
                    IsoCode = "EGY",
                    CurrencyId = null,
                    PhoneNumberLength = 11,
                    CreationDate = DateTime.UtcNow
                });
            }

            if (!Countries.Any(c => c.Code == "TZA"))
            {
                Countries.Add(new Country
                {
                    Code = "TZA",
                    DefaultLanguage = "en",
                    Id = 2,
                    CreatedBy = "System",
                    NameEn = "Tanzania",
                    NameAr = "تنزانيا",
                    AllowMultipleRoles = true,
                    InternationalDialCode = "255",
                    IsoCode = "TZA",
                    CurrencyId = null,
                    PhoneNumberLength = 9,
                    CreationDate = DateTime.UtcNow
                });
            }

            SeedCurrencies();

            if (Countries.Any(c => c.Code == "EGY" && c.CurrencyId == null) && Currencies.Any(s => s.CurrencyCode == "CUR-EGP"))
            {
                var _ = Countries.First(c => c.Code == "EGY");
                _.CurrencyId = Currencies.First(s => s.CurrencyCode == "CUR-EGP").Id;
            }

            if (Countries.Any(c => c.Code == "TZA" && c.CurrencyId == null) && Currencies.Any(s => s.CurrencyCode == "CUR-TZS"))
            {
                var _ = Countries.First(c => c.Code == "TZA");
                _.CurrencyId = Currencies.First(s => s.CurrencyCode == "CUR-TZS").Id;
            }

            SaveChanges();
        }

        void SeedCurrencies()
        {
            bool needSave = false;
            if (Currencies.Where(s => s.Code == "CUR-EGP").Count() == 0)
            {
                Currencies.Add(new Currency(0, "EGP", "CUR-EGP")
                {
                    CurrencyNameAr = "ج.م.",
                });
                needSave = true;
            }

            if (Currencies.Where(s => s.Code == "CUR-TZS").Count() == 0)
            {
                Currencies.Add(new Currency(0, "TSH", "CUR-TZS")
                {
                    CurrencyNameAr = "ش.ت.",
                });
                needSave = true;
            }
            if (needSave)
            {
                SaveChanges();
            }
        }

    }
}
