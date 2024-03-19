using Emeint.Core.BE.API.Infrastructure.Middlewares;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Configurations.Infrastructure.Repositories;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Managers;
using Emeint.Core.BE.SMS.Infractructure.Data;
using Emeint.Core.BE.SMS.Infractructure.Model;
using Emeint.Core.BE.SMS.Infractructure.Repositories;
using Emeint.Core.BE.SMS.InterCommunication.Consumers;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using ConfigurationManager = Emeint.Core.BE.SMS.Domain.Configurations.ConfigurationManager;

namespace Emeint.Core.BE.SMS
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
            });

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



            // Register the Swagger generator
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SMS HTTP API",
                    Version = "v1",
                    Description = "The SMS Service HTTP API",
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
                            Scopes = new Dictionary<string, string>() { { "sms", "SMS Service" } }
                        }
                    },
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Reference"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Emeint.Core.BE.SMS.xml");
                options.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    //.AllowCredentials()
                    .AllowAnyHeader());
            });

            #region Register DBContext's

            services.AddDbContext<SmsDbContext>(options =>
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

            #region Register Repositories and Managers

            //singleton  services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //scoped services
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ISmsMessageManager, SmsMessageManager>();
            services.AddScoped<ISmsMessageManager, SmsMessageManager>();
            services.AddScoped<ISmsMessageRepository, SmsMessageRepository>();
            services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserManager, UserManager>();

            //transient services
            services.AddTransient<IHashingManager, HashingManager>();

            #endregion

            #region Register Consumers
            services.AddScoped<SendSmsQMessageConsumer>();

            InterCommDependenciesConfigurator.ConfigureServices(services, Configuration, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.SendSms", e => { e.Consumer<SendSmsQMessageConsumer>(provider); });
            });

            #endregion

            CacheDependenciesResolver.Register(services, Configuration);

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
                options.ApiName = "sms";
                options.SupportedTokens = SupportedTokens.Reference;
                options.RequireHttpsMetadata = false;
                options.ApiSecret = Configuration["ApisSecret"];
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
                   options.Audience = "sms";
                   //options.TokenValidationParameters = new TokenValidationParameters
                   //{
                   //    ValidateIssuer = true,
                   //    ValidIssuer = identityUrl,
                   //    ValidateAudience = true,
                   //    ValidAudience = "sms",
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
            int retainedLogFileCountLimit = Configuration.GetValue<int>("RetainedLogFileCountLimit");
            if (retainedLogFileCountLimit > 0)
            {
                loggerFactory.AddFile("Logs/Sms-{Date}.txt", retainedFileCountLimit: retainedLogFileCountLimit);
            }
            else
            {
                loggerFactory.AddFile("Logs/Sms-{Date}.txt");
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
            //app.UseMiddleware<RequestLoggingMiddleware<SmsDbContext>>();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SMS API V1");
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
