using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Emeint.Core.BE.API.Infrastructure.Middlewares.LoggingMiddlewares;
using Emeint.Core.BE.Cache.Services.Concretes;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.API.Infrastructure.Filters;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using CacheManager.Core;
using System.IdentityModel.Tokens.Jwt;
using Emeint.Core.BE.API.Infrastructure.Middlewares;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Emeint.Core.BE.Identity.Configuration;
using Emeint.Core.BE.Configurations;
using Emeint.Core.BE.Media;
using Microsoft.OpenApi.Models;

namespace Emeint.Core.BE.Identity
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
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            var connectionString = Configuration["ConnectionString"];
            var identityUrl = Configuration["IdentityUrl"];

            #region Commented
            //  services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(connectionString,
            //                        sqlServerOptionsAction: sqlOptions =>
            //                        {
            //                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //                        }));

            //  services.AddDbContext<ConfigurationContext>(options =>
            //  {
            //      options.UseSqlServer(connectionString,
            //          sqlServerOptionsAction: sqlOptions =>
            //          {
            //              sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //          });
            //  },
            //     ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            //);

            //  services.AddEntityFrameworkSqlServer()
            //  .AddDbContext<CountryContext>(options =>
            //  {
            //      options.UseSqlServer(connectionString,
            //                         sqlServerOptionsAction: sqlOptions =>
            //                         {
            //                             sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //                         });
            //  },
            //  ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            //  );

            //  #region Add ImageContext
            //  services.AddDbContext<ImageContext>(options =>
            //  {
            //      options.UseSqlServer(connectionString,
            //          sqlServerOptionsAction: sqlOptions =>
            //          {
            //              sqlOptions.MigrationsAssembly(typeof(Media.Startup).GetTypeInfo().Assembly.GetName().Name);
            //              sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //          });
            //  },
            //      ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)

            //  );
            //  #endregion


            //var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            //{
            //    config.Password.RequireNonAlphanumeric = false;
            //    config.Password.RequireLowercase = false;
            //    config.Password.RequireUppercase = false;
            //    config.Password.RequireDigit = false;
            //    config.Password.RequiredLength = 1;
            //    config.Password.RequiredUniqueChars = 0;
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();


            //var cert = new X509Certificate2(@"Certificate\identity.pfx", "EMEint123", X509KeyStorageFlags.MachineKeySet);
            //services.AddIdentityServer()
            //            .AddSigningCredential(cert)
            //            .AddAspNetIdentity<ApplicationUser>()
            //            .AddConfigurationStore(options =>
            //            {
            //                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
            //                     sqlServerOptionsAction: sqlOptions =>
            //                     {
            //                         sqlOptions.MigrationsAssembly(migrationsAssembly);
            //                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //                     });
            //            })
            //            .AddOperationalStore(options =>
            //            {
            //                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
            //                    sqlServerOptionsAction: sqlOptions =>
            //                    {
            //                        sqlOptions.MigrationsAssembly(migrationsAssembly);
            //                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
            //                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //                    });
            //            })
            //                                        .Services.AddTransient<IProfileService, ProfileService>()
            //                                        .AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //});

            // SnakeCase
            /*services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));

            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
                };
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });    */

            //services.AddScoped<IApplicationVersionRepository, ApplicationVersionRepository>();
            //services.AddScoped<IIdentityConfigurationManager, IdentityConfigurationManager>();
            //services.AddScoped<IIdentityService, IdentityService>();
            //services.AddScoped<ILogger, Logger<IdentityFilter>>();
            //services.AddScoped<IdentityFilter>();
            //services.AddScoped<ICountryRepository, CountryRepository>();
            //services.AddTransient<ICountryService, CountryService>();
            //services.AddScoped<IProfileRepository, ProfileRepository>();
            //services.AddScoped<IImageRepository, ImageRepository>();
            //services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
            //services.AddTransient<IRedirectService, RedirectService>();
            //services.AddTransient<IHashingManager, HashingManager>();
            //services.AddScoped<ISettingRepository, SettingRepository>();

            #endregion 

            IdentityDependenciesResolver.Register(services, Configuration);
            ConfigurationsDependenciesResolver.Register(services, Configuration);
            MediaDependenciesResolver.Register(services, Configuration);

            services.AddSingleton(typeof(ICacheManagerConfiguration),
             new CacheManager.Core.ConfigurationBuilder()
                     .WithJsonSerializer()
                     .WithMicrosoftMemoryCacheHandle()
                     .Build());
            services.AddSingleton(typeof(CacheManager.Core.ICache<>), typeof(BaseCacheManager<>));
            services.AddScoped(typeof(ICacheService), typeof(MemoryCacheService));
            services.AddApiVersioning();
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Core Be Identity HTTP API",
                    Version = "v1",
                    Description = "The Core Be Identity HTTP API",
                    TermsOfService = null //"Terms Of Service"
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Emeint.Core.BE.Identity.xml");
                //options.IncludeXmlComments(xmlPath);

                /*options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{identityUrl}/connect/authorize",
                    TokenUrl = $"{identityUrl}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "Identity", "Identity API" }
                    }
                });*/

                options.OperationFilter<AuthorizeCheckOperationFilter>();
                /* not need in swagger 5 and openId 3*/
                /*options.OperationFilter<FileOperation>();*/

            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddScoped<IIdentityDataConfig, Config>();

            //ConfigureAuthService(services);           
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
            int retainedLogFileCountLimit = Configuration.GetValue<int>("RetainedLogFileCountLimit");
            if (retainedLogFileCountLimit > 0)
            {
                loggerFactory.AddFile("Logs/CoreBeIdentity-{Date}.txt", retainedFileCountLimit: retainedLogFileCountLimit);
            }
            else
            { 
                loggerFactory.AddFile("Logs/CoreBeIdentity-{Date}.txt");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var pathBase = Configuration["PATH_BASE"];
            app.UseSwagger()
          .UseSwaggerUI(c =>
          {
              c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "CoreBeIdentity.API V1");
          });
            app.UseHttpsRedirection();

            // Adds IdentityServer
            app.UseIdentityServer();
            app.UseAuthentication();

            // to serve static file like HTML
            //app.UseStaticFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
            app.UseStaticFiles("/wwwroot");

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });

            app.UseMiddleware<ExceptionCatchMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
        }
        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration["IdentityUrl"];

            var cert = new X509Certificate2(@"Certificate\identity.pfx", "EMEint123", X509KeyStorageFlags.MachineKeySet);
            SecurityKey key = new X509SecurityKey(cert);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "identity";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = identityUrl,
                    ValidateAudience = true,
                    ValidAudience = "identity",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(
                    //Encoding.UTF8.GetBytes(Configuration["SecurityKey"])),
                    RoleClaimType = "role",
                    IssuerSigningKey = key,
                };
            });
        }

    }

}