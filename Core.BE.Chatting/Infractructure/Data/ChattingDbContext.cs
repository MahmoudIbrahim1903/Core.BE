using Emeint.Core.BE.Chatting.Infractructure.EntityConfigurations;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Infractructure.Data
{
    public class ChattingDbContext : DbContext
    {
        public ChattingDbContext()
        {

        }
        public ChattingDbContext(DbContextOptions<ChattingDbContext> options) : base(options)
        {

        }
        public static string DEFAULT_SCHEMA = "Chatting";

        public DbSet<Channel> Channels { set; get; }
        public DbSet<ChannelMember> ChannelMembers { set; get; }
        public DbSet<User> Users { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ChannelEntityConfigurations());
            modelBuilder.ApplyConfiguration(new ChannelMemberEntityConfigurations());
            modelBuilder.ApplyConfiguration(new UserEntityConfigurations());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            optionsBuilder.UseSqlServer(configuration["ConnectionString"], sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });

            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
