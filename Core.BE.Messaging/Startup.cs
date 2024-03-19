using Autofac;
using Autofac.Extensions.DependencyInjection;
using Emeint.Core.BE.API.Infrastructure.Filters;
using Emeint.Core.BE.API.Infrastructure.Middlewares;
using Emeint.Core.BE.API.Infrastructure.Middlewares.LoggingMiddlewares;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Configurations.Infrastructure.Repositories;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.Notifications.API.Application.IntegrationEvents;
using Emeint.Core.BE.Notifications.API.Filters;
using Emeint.Core.BE.Notifications.API.InterCommunication.Consumers;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.PlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Configurations;
using Emeint.Core.BE.Notifications.Domain.Manager;
using Emeint.Core.BE.Notifications.Domain.Manager.Concretes;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using Emeint.Core.BE.Notifications.Infrastructure;
using Emeint.Core.BE.Notifications.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders.Azure;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders.FireBase;
using Emeint.Core.BE.Notifications.Services.Serivces.Concretes;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.BuildingBlocks.EventBus;
using Microsoft.BuildingBlocks.EventBus.Abstractions;
using Microsoft.BuildingBlocks.EventBusServiceBus;
using Microsoft.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.BuildingBlocks.IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;

namespace Emeint.Core.BE.Notifications
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        //:base(configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
           .AddNewtonsoftJson(options =>
           {
               options.SerializerSettings.ContractResolver = new DefaultContractResolver
               {
                   NamingStrategy = new SnakeCaseNamingStrategy()
               };
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

            services.AddDbContext<MessagingContext>(options =>
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

            services.AddHealthChecks();
            //services.AddHealthChecks(checks =>
            //{
            //    var minutes = 1;
            //    if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
            //    {
            //        minutes = minutesParsed;
            //    }
            //    checks.AddSqlCheck("MessagingDb", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            //});       

            //services.AddScoped<MessagingViewsFilter>();
            //API Versioning 
            //services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));

            // Configure GracePeriodManager Hosted Service
            //services.AddSingleton<IHostedService, GracePeriodManagerService>();
            #region Add Image Repository Registration
            //services.AddScoped<IImageRepository, ImageRepository>();
            #endregion
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageStatusRepository, MessageStatusRepository>();
            services.AddScoped<IMessageTypeRepository, MessageTypeRepository>();
            services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddScoped<IDevicePlatformRepository, DevicePlatformRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
            services.AddScoped<WhiteListFilter>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IConfigurationManager, Emeint.Core.BE.Notifications.Domain.Configurations.ConfigurationManager>();
            services.AddScoped<IHashingManager, HashingManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMessageDestinationRepository, MessageDestinationRepository>();
            services.AddScoped<IMessageAggregateService, MessageAggregateService>();
            services.AddScoped<IAdminMessageService, AdminMessageService>();
            services.AddScoped<IEndUserMessageService, EndUserMessageService>();
            services.AddScoped<IServerMessageService, ServerMessageService>();

            if (Configuration["PushNotificationProvider"].ToLower() == "azure")
                services.AddScoped<IMobilePushNotificationProvider, AzureMobilePushNotificationProvider>();
            else if (Configuration["PushNotificationProvider"].ToLower() == "firebase")
                services.AddScoped<IMobilePushNotificationProvider, FirebaseMobileNotificationProvider>();

            services.AddScoped<IWebBrowserPushNotificationProvider, FirebaseWebBrowserNotificationProvider>();

            #region Register Consumers
            services.AddScoped<SendNotificationQMessageConsumer>();
            services.AddScoped<RegisterUserQMessageConsumer>();

            InterCommDependenciesConfigurator.ConfigureServices(services, Configuration, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.SendPushNotification", e => { e.Consumer<SendNotificationQMessageConsumer>(provider); });
                cfg.ReceiveEndpoint("EntityChanges.UserToMessaging", e => { e.Consumer<RegisterUserQMessageConsumer>(provider); });
            });

            #endregion
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.UseApiBehavior = false;
            });

            // Configure Swagger
            services.AddSwaggerGen(options =>
            {
                /*  options.DescribeAllEnumsAsStrings(); */
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Messaging HTTP API",
                    Version = "v1",
                    Description = "The Messaging Service HTTP API",
                    TermsOfService = null
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
                            Scopes = new Dictionary<string, string>() { { "messaging", "Messaging Service" } }
                        }
                    },
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Reference"
                });
                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Emeint.Core.BE.Notifications.xml");
                options.IncludeXmlComments(xmlPath);

                /*  options.AddSecurityDefinition("oauth2",
                new OAuth2Scheme
               {
                   Type = "oauth2",
                   Flow = "password",
                   AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/authorize",
                   TokenUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/token",
                   Scopes = new Dictionary<string, string>()
                   {
                       { "messaging", "Messaging API" }
                   }
               }

                   );*/

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
                    //.AllowCredentials()
                    );
            });

            // Add application services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<IMessagingIntegrationEventService, MessagingIntegrationEventService>();

            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

                    var serviceBusConnectionString = Configuration["EventBusConnection"];
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
                });
            }
            else
            {
                //    //services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                //    //{
                //    //    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                //    //    var factory = new ConnectionFactory()
                //    //    {
                //    //        HostName = Configuration["EventBusConnection"]
                //    //    };

                //    //    if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                //    //    {
                //    //        factory.UserName = Configuration["EventBusUserName"];
                //    //    }

                //    //    if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                //    //    {
                //    //        factory.Password = Configuration["EventBusPassword"];
                //    //    }

                //    //    var retryCount = 5;
                //    //    if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                //    //    {
                //    //        retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                //    //    }

                //    //    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                //    //});
            }
            services.AddHealthChecks();

            RegisterEventBus(services);

            ConfigureAuthService(services, Configuration);
            services.AddOptions();

            //configure autofac
            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory, MessagingContext db)
        {
            //db.Database.Migrate();

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug(LogLevel.Trace);
            //loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
            int retainedLogFileCountLimit = Configuration.GetValue<int>("RetainedLogFileCountLimit");
            if (retainedLogFileCountLimit > 0)
            {
                loggerFactory.AddFile("Logs/Messaging-{Date}.txt", retainedFileCountLimit: retainedLogFileCountLimit);
            }
            else
            {
                loggerFactory.AddFile("Logs/Messaging-{Date}.txt");
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
            app.UseMiddleware<RequestLoggingMiddleware>();
            ConfigureAuth(app);

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseSwagger().UseSwaggerUI(c =>
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
                options.ApiName = "messaging";
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
                   options.Audience = "messaging";
                   //options.TokenValidationParameters = new TokenValidationParameters
                   //{
                   //    ValidateIssuer = true,
                   //    ValidIssuer = identityUrl,
                   //    ValidateAudience = true,
                   //    ValidAudience = "messaging",
                   //    ValidateLifetime = true,
                   //    ValidateIssuerSigningKey = true,
                   //    //IssuerSigningKey = new SymmetricSecurityKey(
                   //    //Encoding.UTF8.GetBytes(Configuration["SecurityKey"])),
                   //    RoleClaimType = "role",
                   //    IssuerSigningKey = key,
                   //};
               });
            }


            //var TokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuerSigningKey = true,
            //    ValidateIssuer = true,
            //    ValidIssuer = identityUrl,
            //    IssuerSigningKey = new X509SecurityKey(cert),
            //};
            //services.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    Authority = identityUrl,
            //    Audience = "WebAPI",
            //    RequireHttpsMetadata = false,
            //    TokenValidationParameters = TokenValidationParameters
            //});
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
                //services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                //{
                //    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                //    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                //    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                //    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                //    var retryCount = 5;
                //    if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                //    {
                //        retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                //    }

                //    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                //});
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}
