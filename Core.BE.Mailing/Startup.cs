using Autofac;
using Autofac.Extensions.DependencyInjection;
using Emeint.Core.BE.API.Infrastructure.Filters;
using Emeint.Core.BE.API.Infrastructure.Middlewares;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Configurations.Infrastructure.Repositories;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.Mailing.API.Filters;
using Emeint.Core.BE.Mailing.API.InterCommunication.Consumers;
using Emeint.Core.BE.Mailing.Domain.Configurations;
using Emeint.Core.BE.Mailing.Domain.Managers;
using Emeint.Core.BE.Mailing.Infrastructure;
using Emeint.Core.BE.Mailing.Infrastructure.Repositories;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.BuildingBlocks.EventBus;
using Microsoft.BuildingBlocks.EventBus.Abstractions;
using Microsoft.BuildingBlocks.EventBusServiceBus;
using Microsoft.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.BuildingBlocks.IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using ConfigurationManager = Emeint.Core.BE.Mailing.Domain.Configurations.ConfigurationManager;

//using Microsoft.AspNetCore.Mvc.Versioning;

namespace Emeint.Core.BE.Mailing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                //options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
           .AddNewtonsoftJson(options =>
           {
               options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
               {
                   NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
               };
               options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
           });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //RegisterAppInsights(services);

            #region Add IFileProvider
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
            #endregion

            // Add cache service provider
            CacheDependenciesResolver.Register(services, Configuration);

            // Add framework services.

            services.AddDbContext<MailingContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        //sqlOptions.UseRowNumberForPaging();
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
            ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });



            services.AddScoped<SendingEmailQMessageConsumer>();

            InterCommDependenciesConfigurator.ConfigureServices(services, Configuration, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.SendEmail", e => { e.Consumer<SendingEmailQMessageConsumer>(provider); });
            });

            #region Add ConfigurationContext
            services.AddDbContext<ConfigurationContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
                ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );
            #endregion

            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();

            //services.AddHealthChecks(checks =>
            //{
            //    var minutes = 1;
            //    if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
            //    {
            //        minutes = minutesParsed;
            //    }
            //    checks.AddSqlCheck("MessagingDb", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            //});


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

            // Configure Swagger
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Mailing HTTP API",
                    Version = "v1",
                    Description = "The Mailing Service HTTP API",
                    TermsOfService = null // "Terms Of Service"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration["IdentityUrl"]}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["IdentityUrl"]}/connect/token"),
                            Scopes = new Dictionary<string, string>()
                           {
                               { "mailing", "Mailing Service" }
                           }
                        }
                    },
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Reference"
                });
                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Emeint.Core.BE.Mailing.xml");
                options.IncludeXmlComments(xmlPath);


                /* options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                 {
                     Type = "oauth2",
                     Flow = "password",
                     AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/authorize",
                     TokenUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/token",
                     Scopes = new Dictionary<string, string>()
                     {
                         { "mailing", "Mailing API" }
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
                   // .AllowCredentials()
                   );
            });

            // Add application services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IEmailManager, SendGridEmailManager>();
            services.AddTransient<IHashingManager, HashingManager>();

            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));


            services.AddScoped<IMailRepository, MailRepository>();
            services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
            services.AddScoped<WhiteListFilter>();

            //services.AddTransient<IMessagingIntegrationEventService, MessagingIntegrationEventService>();


            //if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            //{
            //    services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            //    {
            //        var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

            //        var serviceBusConnectionString = Configuration["EventBusConnection"];
            //        var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

            //        return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            //    });
            //}
            //else
            //{

            //}

            RegisterEventBus(services);
            ConfigureAuthService(services, Configuration);
            services.AddOptions();
            services.AddHealthChecks();
            //configure autofac

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory, MailingContext db)
        {
            db.Database.Migrate();

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug(LogLevel.Trace);
            //loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
            int retainedLogFileCountLimit = Configuration.GetValue<int>("RetainedLogFileCountLimit");
            if (retainedLogFileCountLimit > 0)
            {
                loggerFactory.AddFile("Logs/Mailing-{Date}.txt", retainedFileCountLimit: retainedLogFileCountLimit);
            }
            else
            {
                loggerFactory.AddFile("Logs/Mailing-{Date}.txt");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseMiddleware<ExceptionCatchMiddleware>();
            //app.UseMiddleware<RequestLoggingMiddleware<MailingContext>>();
            ConfigureAuth(app);


            #region Use the Middleware
            //app.UseImageDownloader();
            #endregion


            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "Messaging.API V1");
               });

            app.UseStaticFiles();
            //ConfigureEventBus(app);

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });
        }

        //private void RegisterAppInsights(IServiceCollection services)
        //{
        //    services.AddApplicationInsightsTelemetry(Configuration);
        //    var orchestratorType = Configuration.GetValue<string>("OrchestratorType");

        //    if (orchestratorType?.ToUpper() == "K8S")
        //    {
        //        // Enable K8s telemetry initializer
        //        services.EnableKubernetes();
        //    }
        //    if (orchestratorType?.ToUpper() == "SF")
        //    {
        //        // Enable SF telemetry initializer
        //        services.AddSingleton<ITelemetryInitializer>((serviceProvider) =>
        //            new FabricTelemetryInitializer());
        //    }
        //}

        //private void ConfigureEventBus(IApplicationBuilder app)
        //{
        //    var eventBus = app.ApplicationServices.GetRequiredService<Microsoft.BuildingBlocks.EventBus.Abstractions.IEventBus>();

        //    //eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
        //    //eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
        //    //eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>>();
        //    //eventBus.Subscribe<OrderStockRejectedIntegrationEvent, IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>>();
        //    //eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>>();
        //    //eventBus.Subscribe<OrderPaymentSuccededIntegrationEvent, IIntegrationEventHandler<OrderPaymentSuccededIntegrationEvent>>();
        //}

        private void ConfigureAuthService(IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            if (Configuration["AccessTokenType"] == "Reference")
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = identityUrl;
                options.ApiName = "mailing";
                options.SupportedTokens = SupportedTokens.Reference;
                options.RequireHttpsMetadata = false;
                options.ApiSecret = configuration["ApisSecret"];
            });
            }
            else
            {
                //var cert = new X509Certificate2(Configuration["Certificate"], Configuration["CertificatePassword"], X509KeyStorageFlags.MachineKeySet);
                //SecurityKey key = new X509SecurityKey(cert);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
               .AddJwtBearer(options =>
               {
                   options.Authority = identityUrl;
                   options.RequireHttpsMetadata = false;
                   options.Audience = "mailing";
                   //options.TokenValidationParameters = new TokenValidationParameters
                   //{
                   //    ValidateIssuer = true,
                   //    ValidIssuer = identityUrl,
                   //    ValidateAudience = true,
                   //    ValidAudience = "mailing",
                   //    ValidateLifetime = true,
                   //    ValidateIssuerSigningKey = true,
                   //    //IssuerSigningKey = new SymmetricSecurityKey(
                   //    //Encoding.UTF8.GetBytes(Configuration["SecurityKey"])),
                   //    RoleClaimType = "role",
                   //    IssuerSigningKey = key,
                   //};
               });
            }
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();
        }

        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];

            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
                });
            }
            else
            {

            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}
