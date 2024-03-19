using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Emeint.Core.BE.API.Infrastructure.Filters;
using Emeint.Core.BE.API.Infrastructure.Middlewares;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Concretes;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Configurations.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer4.AccessTokenValidation;
using Microsoft.EntityFrameworkCore;
using Emeint.Core.BE.Chatting.Infractructure.Data;
using System.Reflection;
using Emeint.Core.BE.Chatting.InterCommunication.Consumers;
using Emeint.Core.BE.InterCommunication;
using MassTransit;
using Emeint.Core.BE.Chatting.Domain.ChattingProviders;
using Emeint.Core.BE.Chatting.Domain.Configurations;
using Emeint.Core.BE.Chatting.Services.Contracts;
using Emeint.Core.BE.Chatting.Services.Concretes;
using Emeint.Core.BE.Chatting.Infractructure.IRepositories;
using Emeint.Core.BE.Chatting.Infractructure.Repositories;
using Emeint.Core.BE.Utilities;
using GreenPipes;

namespace Emeint.Core.BE.Chatting
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

            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
                };
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

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

            // Register the Swagger generator
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Chatting HTTP API",
                    Version = "v1",
                    Description = "The Chatting Service HTTP API",
                    TermsOfService = null //"Terms Of Service"
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
                              { "chatting", "Chatting Service" }
                           }
                        }
                    },
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Reference"
                });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            #region Register DBContext's

            services.AddDbContext<ChattingDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        //sqlOptions.UseRowNumberForPaging();
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
               ServiceLifetime.Scoped
               );

            services.AddDbContext<ConfigurationContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
                ServiceLifetime.Scoped
            );

            #endregion

            #region Register Repositories, services and Managers

            //singleton
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //scoped
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IChannelRepository, ChannelRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatProvider, SendbirdProvider>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();

            #endregion

            #region Register Consumers
            services.AddScoped<CreateChannelQMessageConsumer>();
            services.AddScoped<ChattingProfileChangeQMessageConsumer>();

            InterCommDependenciesConfigurator.ConfigureServices(services, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.CreateChatChannel", e =>
                {
                    e.Consumer<CreateChannelQMessageConsumer>(provider);
                    e.UseMessageRetry(r => r.Interval(Configuration.GetValue<int>("QueueRetryCount"),
                           TimeSpan.FromSeconds(Configuration.GetValue<int>("QueueRetryIntervalInSeconds"))));
                });

                cfg.ReceiveEndpoint("ReplicatedColumnsChanges.ChattingUserProfile", e => { e.Consumer<ChattingProfileChangeQMessageConsumer>(provider); });
            });

            #endregion

            services.AddSingleton(typeof(ICacheManagerConfiguration),
              new CacheManager.Core.ConfigurationBuilder()
                      .WithJsonSerializer()
                      .WithMicrosoftMemoryCacheHandle()
                      .Build());
            services.AddSingleton(typeof(CacheManager.Core.ICache<>), typeof(BaseCacheManager<>));
            services.AddScoped(typeof(ICacheService), typeof(MemoryCacheService));

            //Core Services
            services.AddScoped<INetworkCommunicator, NetworkCommunicator>();
            services.AddScoped<IWebRequestUtility, WebRequestUtility>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHealthChecks();
            ConfigureAuthService(services);
        }
        private void ConfigureAuthService(IServiceCollection services)
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
                options.ApiName = "chatting";
                options.SupportedTokens = SupportedTokens.Reference;
                options.RequireHttpsMetadata = false;
                options.ApiSecret = Configuration["ApisSecret"];
            });
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
               .AddJwtBearer(options =>
               {
                   options.Authority = identityUrl;
                   options.RequireHttpsMetadata = false;
                   options.Audience = "chatting";
               });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
            int retainedLogFileCountLimit = Configuration.GetValue<int>("RetainedLogFileCountLimit");
            if (retainedLogFileCountLimit > 0)
            {
                loggerFactory.AddFile("Logs/Chatting-{Date}.txt", retainedFileCountLimit: retainedLogFileCountLimit);
            }
            else 
            { 
                loggerFactory.AddFile("Logs/Chatting-{Date}.txt");
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

            app.UseCors("CorsPolicy");
            app.UseMiddleware<ExceptionCatchMiddleware>();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chatting API V1");
            });

            app.UseHttpsRedirection();
            ConfigureAuth(app);

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });

        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();
        }
    }
}
