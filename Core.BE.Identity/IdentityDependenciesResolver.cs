using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Identity.API.Filters;
using Emeint.Core.BE.Identity.API.Infrastructure.CustomGrant;
using Emeint.Core.BE.Identity.CustomizedTokenProvider;
using Emeint.Core.BE.Identity.Domain.Configurations;
using Emeint.Core.BE.Identity.Domain.Managers;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Infrastructure.Services.Concretes;
using Emeint.Core.BE.Identity.Infrastructure.Services.Concretes.InterCommunication;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts.InterCommunication;
using Emeint.Core.BE.Identity.Services;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.Utilities;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Emeint.Core.BE.Identity
{
    public static class IdentityDependenciesResolver
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];
            var identityUrl = configuration["IdentityUrl"];

            var migrationsAssembly = typeof(IdentityDependenciesResolver).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomPhoneNumberTokenProvider>(CustomPhoneNumberTokenProvider.Phone);//used to custom phone token length(4), comment to use the default length(6). 


            var cert = new X509Certificate2(configuration["Certificate"], configuration["CertificatePassword"], X509KeyStorageFlags.MachineKeySet);
            services.AddIdentityServer()
                        .AddSigningCredential(cert)
                        .AddAspNetIdentity<ApplicationUser>()
                        .AddConfigurationStore(options =>
                        {
                            options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                 sqlServerOptionsAction: sqlOptions =>
                                 {
                                     sqlOptions.MigrationsAssembly(migrationsAssembly);
                                     sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                 });
                        })
                        .AddOperationalStore(options =>
                        {
                            options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                sqlServerOptionsAction: sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                        })
                        .Services.AddTransient<IProfileService, ProfileService>()
                        .AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();


            services.AddScoped<IApplicationVersionRepository, ApplicationVersionRepository>();
            services.AddScoped<IIdentityConfigurationManager, IdentityConfigurationManager>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ILogger, Logger<IdentityFilter>>();
            services.AddScoped<IdentityFilter>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
            services.AddTransient<IHashingManager, HashingManager>();
            services.AddScoped<INetworkCommunicator, NetworkCommunicator>();
            services.AddScoped<IWebRequestUtility, WebRequestUtility>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAdminsService, AdminsService>();
            services.AddScoped<IClientRoleValidator, ClientRoleValidator>();
            services.AddScoped<IAdminsRepository, AdminsRepository>();
            services.AddScoped<ISendEmailNotifierService, SendEmailNotifierService>();
            //services.AddScoped<ISendSmsNotifierService, SendSmsNotifierService>();
            services.AddScoped<IPersistedGrantRepository, PersistedGrantRepository>();
            services.AddScoped<IPermittedCountryService, PermittedCountryService>();
            services.AddScoped<IPermittedCountryRepository, PermittedCountryRepository>();
            services.AddScoped<ITokenRevocationResponseGenerator, CustomTokenRevocationResponseGenerator>();

            ConfigureAuthService(services, configuration);
            InterCommDependenciesConfigurator.ConfigureServices(services, configuration);
        }

        private static void ConfigureAuthService(IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = configuration["IdentityUrl"];
            if (configuration["AccessTokenType"] == "Reference")
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = identityUrl;
                options.ApiName = "identity";
                options.SupportedTokens = SupportedTokens.Reference;
                options.RequireHttpsMetadata = false;
                options.ApiSecret = configuration["ApisSecret"];
            });
            }
            else
            {
                var cert = new X509Certificate2(configuration["Certificate"], configuration["CertificatePassword"], X509KeyStorageFlags.MachineKeySet);
                SecurityKey key = new X509SecurityKey(cert);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
               .AddJwtBearer(options =>
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
}
