using Castle.Compatibility;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data.EnitityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Emeint.Core.BE.Identity.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        protected readonly IConfiguration _configuration;
        public DbSet<ApplicationVersion> ApplicationVersions { get; set; }
        public DbSet<PermittedCountry> PermittedCountries { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public virtual void Seed()
        {

            //Add Roles
            List<string> roleList = new List<string>{
                "SuperAdmin",
                "ConfigurationsReader","ConfigurationsFullController",
                "VersionsReader", "VersionsFullController",
                "AdministratorsReader", "AdministratorsFullController",
                "NotificationsReader","NotificationsFullController"
            };

            roleList.Where(role => !Roles.AnyAsync(r => r.Name == role).Result)
                    .ForEach(role => Roles.Add(new ApplicationRole(role) { IsAdmin = true, NormalizedName = role.ToUpper() }));

            //foreach (var role in roleList)
            //{
            //    if (!Roles.AnyAsync(r => r.Name == role).Result)
            //    {
            //        Roles.Add(new ApplicationRole(role) { IsAdmin = true });
            //    }
            //}

            //SaveChanges();
            //Roles.ForEach(r => r.NormalizedName = r.Name.ToUpper());
            SaveChanges();
        }

        // to be used if we incounter an error ( No database provider has been configured for this DbContext)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                var connectionString =
                    _configuration["ConnectionString"];

                optionsBuilder.UseSqlServer(connectionString);
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ApplicationVersionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PermittedCountryEntityConfiguration());

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationRole>()
                .HasMany(e => e.Users)
                .WithOne()
                .HasForeignKey(e => e.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(e => e.SuspensionStatus);
        }
    }
}