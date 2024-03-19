using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Infrastructure.DBContext;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.ApplicationAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Notifications.Infrastructure
{
    public class MessagingContext : CountryBaseDBContext
    {
        private readonly IConfiguration _configuration;

        public const string DEFAULT_SCHEMA = "Messaging";
        public DbSet<ClientApplicationVersion> ClientApplicationVersion { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<MessageDestination> MessageDestination { get; set; }
        public DbSet<MessageStatus> MessageStatus { get; set; }
        public DbSet<MessageExtraParam> MessageExtraParam { get; set; }
        public DbSet<MessageTemplate> MessageTemplate { get; set; }
        public DbSet<MessageType> MessageType { get; set; }
        public DbSet<DevicePlatform> DevicePlatform { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserDevice> UserDevice { get; set; }
        //public DbSet<UserTag> UserTag { get; set; }

        public MessagingContext(DbContextOptions<DbContext> options, IIdentityService identityService, IConfiguration configuration) 
            : base(options, configuration, identityService)
        {
            _configuration = configuration;
        }
        public MessagingContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Apply Configuration for each entity like the example below
            modelBuilder.ApplyConfiguration(new ClientApplicationVersionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageDestinationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageExtraParamEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageTemplateEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DevicePlatformEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserDeviceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            //modelBuilder.ApplyConfiguration(new UserTagEntityTypeConfiguration());
        }
        public virtual void Seed()
        {
            SaveChanges();
        }

        //public class MessagingContextDesignFactory : IDesignTimeDbContextFactory<MessagingContext>
        //{
        //    public MessagingContext CreateDbContext(string[] args)
        //    {
        //        // TODO: from configuration
        //        var optionsBuilder = new DbContextOptionsBuilder<MessagingContext>()
        //        //.UseSqlServer("tcp:dryve.database.windows.net,1433;Initial Catalog=Dryve.Microservice.MessagingDb;Persist Security Info=False;User ID=dryvedb;Password=@@EMEint123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        //        .UseSqlServer("Server=.;Initial Catalog=Microservice.MessagingDb;Integrated Security=true");

        //        return new MessagingContext(optionsBuilder.Options, new NoMediator());
        //    }
        //}

        //class NoMediator : IMediator
        //{
        //    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
        //    {
        //        return Task.CompletedTask;
        //    }

        //    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
        //    {
        //        return Task.FromResult<TResponse>(default(TResponse));
        //    }

        //    public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
        //    {
        //        return Task.CompletedTask;
        //    }
        //}
    }
}
