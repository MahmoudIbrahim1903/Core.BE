using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Gateway.Aggregators;
using Emeint.Core.BE.Gateway.Domain.Contracts;
using Emeint.Core.BE.Gateway.Domain.Services;
using Emeint.Core.BE.Gateway.HttpAgrregators.Proxies;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.Utilities;
using Gateway.Domain.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Ocelot.DependencyInjection;
using System;

namespace Emeint.Core.BE.Gateway.Utilities
{
    public static class GatewayDependenciesResolverUtility
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();

            services.AddControllers(options =>
            {
                //options.Filters.Add(typeof(HttpGlobalExceptionFilter));

            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddApiVersioning(o =>
            {
                o.UseApiBehavior = false;
            });

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddOcelot()
                    .AddTransientDefinedAggregator<ConfigSettingsAggregator>()
                    .AddTransientDefinedAggregator<AdminConfigSettingsAggregator>()
                    .AddTransientDefinedAggregator<AdminManagedConfigSettingsAggregator>();
            //Core Services
            services.AddScoped<INetworkCommunicator, NetworkCommunicator>();
            services.AddScoped<IWebRequestUtility, WebRequestUtility>();
            services.AddHealthChecks()
               .AddUrlGroup(new Uri(configuration["IdentityUrl"] + "/healthcheck"), "Identity");

            ApplicationInsightsServiceOptions aiOptions = new ApplicationInsightsServiceOptions
            {
                EnableAdaptiveSampling = false, // Disables adaptive sampling.
                EnableQuickPulseMetricStream = false, // Disables QuickPulse (Live Metrics stream).
                EnablePerformanceCounterCollectionModule = false
            };
            services.AddApplicationInsightsTelemetry(aiOptions);

            InterCommDependenciesConfigurator.ConfigureServices(services, configuration, (provider, cfg) => { });

            #region Register Services
            services.AddScoped<IAggregationService, AggregationService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ISendEmailNotifierService, SendEmailNotifierService>();
            services.AddScoped<IConfigurationsProxy, ConfigurationsProxy>();

            #endregion
        }
    }
}
