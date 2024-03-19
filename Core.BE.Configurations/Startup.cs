using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CacheManager.Core;
using Emeint.Core.BE.API.Infrastructure.Filters;
using Emeint.Core.BE.API.Infrastructure.Middlewares;
using Emeint.Core.BE.API.Infrastructure.Middlewares.LoggingMiddlewares;
using Emeint.Core.BE.Cache.Services.Concretes;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.Configurations.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Emeint.Core.BE.Configurations
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                //options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            ConfigurationsDependenciesResolver.Register(services, Configuration);


            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.UseApiBehavior = false;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });



            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Configurations HTTP API",
                    Version = "v1",
                    Description = "The Configurations Service HTTP API",
                    TermsOfService = null//"Terms Of Service"
                });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // Add framework services.

            services.AddDbContext<ConfigurationContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            //sqlOptions.MigrationsAssembly("Dryve.Microservices.Messaging.Infrastructure.Migrations");
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                },
                ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                );

            //  ConfigureAuthService(services);

            services.AddSingleton(typeof(ICacheManagerConfiguration),
                new CacheManager.Core.ConfigurationBuilder()
                        .WithJsonSerializer()
                        .WithMicrosoftMemoryCacheHandle()
                        .Build());
            services.AddSingleton(typeof(ICache<>), typeof(BaseCacheManager<>));
            services.AddScoped(typeof(ICacheService), typeof(MemoryCacheService));

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionCatchMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            //    ConfigureAuth(app);

            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(Configuration["PATH_BASE"]) ? Configuration["PATH_BASE"] : string.Empty) }/swagger/v1/swagger.json", "Vehicle.API V1");
              });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });
        }
    }


}
