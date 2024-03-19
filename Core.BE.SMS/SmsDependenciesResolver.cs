using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Models;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Managers;
using Emeint.Core.BE.SMS.Infractructure.Model;
using Emeint.Core.BE.SMS.Infractructure.Repositories;
using Emeint.Core.BE.SMS.InterCommunication.Consumers;
using Emeint.Core.BE.Utilities;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationManager = Emeint.Core.BE.SMS.Domain.Configurations.ConfigurationManager;

namespace Emeint.Core.BE.SMS
{
    public class SmsDependenciesResolver
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SQLConnectionStringOptions>();
            //singleton  services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //scoped services
            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ISmsMessageManager, SmsMessageManager>();
            services.AddScoped<ISmsMessageRepository, SmsMessageRepository>();
            services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserManager, UserManager>();

            //transient services
            services.AddTransient<IHashingManager, HashingManager>();
            services.AddScoped<SendSmsQMessageConsumer>();

            //Core Services
            services.AddScoped<INetworkCommunicator, NetworkCommunicator>();
            services.AddScoped<IWebRequestUtility, WebRequestUtility>();

            InterCommDependenciesConfigurator.ConfigureServices(services, configuration, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.SendSms", e => { e.Consumer<SendSmsQMessageConsumer>(provider); });
            });

        }
    }
}
