using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Infrastructure.DBContext;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using Emeint.Core.BE.Mailing.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Infrastructure
{
    public class MailingContext : CountryBaseDBContext
    {
        public const string DEFAULT_SCHEMA = "Mailing";


        //public DbSet<string> HasAttachments { get; set; }//?
        public DbSet<Mail> Mails { get; set; }
        public DbSet<MailTemplate> MailTemplates { get; set; }

        private readonly IMediator _mediator;

        public MailingContext(DbContextOptions<DbContext> options, IConfiguration configuration, IIdentityService identityService)
            : base(options, configuration, identityService)
        {
            //_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            System.Diagnostics.Debug.WriteLine("MailingContext::ctor ->" + GetHashCode());
        }

        public MailingContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Apply Configuration for each entity like the example below
            modelBuilder.ApplyConfiguration(new MailConfiguration());
            modelBuilder.ApplyConfiguration(new MailTemplateConfiguration());
        }
        public virtual void Seed()
        {
            //TODO : add seed data in mail context entity 
            //SaveChanges();
        }
    }

    //public class MailingContextDesignFactory : IDesignTimeDbContextFactory<MailingContext>
    //{
    //    public IConfiguration Configuration { get; }
    //    public IIdentityService IdentityService { get; }
    //    public MailingContextDesignFactory()
    //    {

    //    }
    //    public MailingContextDesignFactory(IConfiguration configuration, IIdentityService identityService)
    //    {
    //        Configuration = configuration;
    //        IdentityService = identityService;
    //    }
    //    public MailingContext CreateDbContext(string[] args)
    //    {
    //        // TODO: from configuration
    //        var optionsBuilder = new DbContextOptionsBuilder<DbContext>()
    //            .UseSqlServer(.GetCountryDBConnectionString());
    //        //.UseSqlServer("tcp:dryve.database.windows.net,1433;Initial Catalog=Dryve.Microservice.MailingDb;Persist Security Info=False;User ID=dryvedb;Password=@@EMEint123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    //        return new MailingContext(optionsBuilder.Options, new NoMediator(),  Configuration, IdentityService);
    //    }
    //}

    class NoMediator : IMediator
    {
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<TResponse>(default(TResponse));
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }

}
