using System.IdentityModel.Tokens.Jwt;
using CacheManager.Core;
using Emeint.Core.BE.API.Infrastructure.Filters;
using Emeint.Core.BE.API.Infrastructure.Middlewares;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache;
using Emeint.Core.BE.Cache.Services.Concretes;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.LiveUpdates.InterCommunication.Consumers;
using Emeint.Core.BE.LiveUpdates.LiveUpdatesHub;
using Emeint.Core.BE.LiveUpdates.Services.Concretes;
using Emeint.Core.BE.LiveUpdates.Services.Contracts;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Emeint.Core.BE.LiveUpdates
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        //:base(configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
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

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowCredentials()
            //        .AllowAnyHeader());
            //});


            CacheDependenciesResolver.Register(services, Configuration);

            #region Register Consumers
            services.AddScoped<ServerPushLiveUpdateQMessageConsumer>();

            InterCommDependenciesConfigurator.ConfigureServices(services, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.PushLiveUpdate", e => { e.Consumer<ServerPushLiveUpdateQMessageConsumer>(provider); });
            });

            #endregion

            #region Register Repositories, services and Managers

            //singleton
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //scoped
            services.AddScoped<IIdentityService, IdentityService>();

            #endregion
            //Core Services
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddHealthChecks();

            ConfigureAuthService(services);
            services.AddSignalR().AddAzureSignalR();
            services.AddScoped<IServerLiveUpdatesService, ServerLiveUpdatesService>();
            services.AddScoped<IGroupsService, GroupsService>();
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
                options.ApiName = "liveUpdates";
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
                   options.Audience = "liveUpdates";
               });
            }
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();
            app.UseMiddleware<ExceptionCatchMiddleware>();
            //app.UseStaticFiles();
            //app.UseDefaultFiles();
            int retainedLogFileCountLimit = Configuration.GetValue<int>("RetainedLogFileCountLimit");
            if (retainedLogFileCountLimit > 0)
            {
                loggerFactory.AddFile("Logs/LiveUpdates-{Date}.txt", retainedFileCountLimit: retainedLogFileCountLimit);
            }
            else 
            {
                loggerFactory.AddFile("Logs/LiveUpdates-{Date}.txt");
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
            //app.UseCors("CorsPolicy");

            //ConfigureAuth(app);
            app.UseRouting();
            app.UseAuthorization();
            app.UseAzureSignalR(config =>
            {
                config.MapHub<BaseHub>("/live_updates");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthcheck");
            });

        }

        //protected virtual void ConfigureAuth(IApplicationBuilder app)
        //{
        //    //if (Configuration.GetValue<bool>("UseLoadTest"))
        //    //{
        //    //    app.UseMiddleware<ByPassAuthMiddleware>();
        //    //}

        //    app.UseAuthentication();
        //}
    }
}
