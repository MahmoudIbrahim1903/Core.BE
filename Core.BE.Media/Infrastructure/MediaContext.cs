using System;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Emeint.Core.BE.Media.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.IO;
using Emeint.Core.BE.Infrastructure.DBContext;
using Emeint.Core.BE.API.Infrastructure.Services;

namespace Emeint.Core.BE.Media.Infrastructure
{
    public class MediaContext : CountryBaseDBContext
    {
        public const string DEFAULT_SCHEMA = "Media";
        public DbSet<ImageMetaData> ImagesMetaData { get; set; }
        public DbSet<ImageData> ImagesData { get; set; }
        public DbSet<Video> Videos { set; get; }
        public DbSet<Audio> Audios { set; get; }
        public DbSet<Document> Documents { set; get; }


        private readonly IMediator _mediator;
        public MediaContext(DbContextOptions<DbContext> options, IConfiguration configuration, IIdentityService identityService) 
            : base(options, configuration, identityService)
        {
            System.Diagnostics.Debug.WriteLine("ImageContext::ctor ->" + GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ImageMetaDataEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ImageDataEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VideoEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AudioEntityTypeConfiguration());
        }

        //public class ImageContextDesignFactory : IDesignTimeDbContextFactory<ImageContext>
        //{
        //    public static IConfiguration Configuration { get; set; }
        //    public ImageContext CreateDbContext(string[] args)
        //    {
        //        var connectionString = Configuration.GetValue<string>("ConnectionString");
        //        var optionsBuilder = new DbContextOptionsBuilder<ImageContext>()
        //            .UseSqlServer(connectionString);

        //        return new ImageContext(optionsBuilder.Options, new NoMediator());
        //    }

        //    class NoMediator : IMediator
        //    {
        //        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
        //        {
        //            return Task.CompletedTask;
        //        }

        //        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
        //        {
        //            return Task.FromResult<TResponse>(default(TResponse));
        //        }

        //        public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
        //        {
        //            return Task.CompletedTask;
        //        }
        //    }
        //}
    }
}
