using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CacheManager.Core;
using Emeint.Core.BE.API.Infrastructure.Filters;
using Emeint.Core.BE.Media.API.Extensions;
using Emeint.Core.BE.Media.API.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace Emeint.Core.BE.Media
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

            }).AddControllersAsServices()
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

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


            // Add an in-memory cache service provider
            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration),
                new CacheManager.Core.ConfigurationBuilder()
                    .WithJsonSerializer()
                    .WithMicrosoftMemoryCacheHandle()
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .Build());

            // Add framework services.  
            // {
            //services.AddEntityFrameworkSqlServer()
            //    .AddDbContext<ImageContext>(options =>
            //        {
            //            options.UseSqlServer(Configuration["ConnectionString"],
            //                sqlServerOptionsAction: sqlOptions =>
            //                {
            //                    //sqlOptions.MigrationsAssembly("Dryve.Microservices.Vehicle.Infrastructure.Migrations");
            //                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //                });
            //        },
            //        ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            //    );
            //   }

            //services.AddImageDownloader();
            //services.AddScoped<IImageRepository, ImageRepository>();

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.UseApiBehavior = false;
            });

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Image HTTP API",
                    Version = "v1",
                    Description = "The Image Service HTTP API",
                    TermsOfService = null //"Terms Of Service"
                });

                // Set the comments path for the Swagger JSON and UI.
                //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Emeint.Core.BE.Media.xml");
                options.IncludeXmlComments(xmlPath);

                //options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                //{
                //    Type = "oauth2",
                //    Flow = "password",
                //    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/authorize",
                //    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/token",
                //    Scopes = new Dictionary<string, string>()
                //    {
                //        { "vehicles", "Vehicle API" }
                //    }
                //});

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

            // Add application services.
            //services.AddTransient<IIdentityService, IdentityService>();

            MediaDependenciesResolver.Register(services, Configuration);

            //ConfigureAuthService(services);
            services.AddOptions();
            services.AddHealthChecks();

            var container = new ContainerBuilder();
            container.Populate(services);
            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
             //loggerFactory.AddFile("Logs/Media-{Date}.txt"); 

            //app.UseMiddleware<RequestLoggingMiddleware<ImageContext>>();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseImageDownloader();

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Image V1");
                    //c.ConfigureOAuth2("Identityswaggerui", "", "", "Identity Swagger UI");
                });
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //private void ConfigureAuthService(IServiceCollection services)
        //{
        //    // prevent from mapping "sub" claim to nameidentifier.
        //    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        //    var identityUrl = Configuration.GetValue<string>("IdentityUrl");
        //    var cert = new X509Certificate2(@"Certificate\emeint_dryve.crt", "EMEint123", X509KeyStorageFlags.MachineKeySet);
        //    //var cert = new X509Certificate2(@"Certificate\emeint_dryve.pfx", "EMEint123");
        //    SecurityKey key = new X509SecurityKey(cert);

        //    services.AddAuthentication(options =>
        //    {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        //    }).AddJwtBearer(options =>
        //    {
        //        options.Authority = identityUrl;
        //        options.RequireHttpsMetadata = false;
        //        options.Audience = "vehicles";
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidIssuer = identityUrl,
        //            ValidateAudience = true,
        //            ValidAudience = "vehicles",
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            //IssuerSigningKey = new SymmetricSecurityKey(
        //            //Encoding.UTF8.GetBytes(Configuration["SecurityKey"])),
        //            RoleClaimType = "role",
        //            IssuerSigningKey = key,
        //        };
        //    });

        //    //var TokenValidationParameters = new TokenValidationParameters
        //    //{
        //    //    ValidateIssuerSigningKey = true,
        //    //    ValidateIssuer = true,
        //    //    ValidIssuer = identityUrl,
        //    //    IssuerSigningKey = new X509SecurityKey(cert),
        //    //};
        //    //services.UseJwtBearerAuthentication(new JwtBearerOptions
        //    //{
        //    //    Authority = identityUrl,
        //    //    Audience = "WebAPI",
        //    //    RequireHttpsMetadata = false,
        //    //    TokenValidationParameters = TokenValidationParameters
        //    //});

        //}

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
    }
}
