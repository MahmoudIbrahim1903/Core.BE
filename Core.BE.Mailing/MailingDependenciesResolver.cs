using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Models;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.Mailing.API.Filters;
using Emeint.Core.BE.Mailing.API.InterCommunication.Consumers;
using Emeint.Core.BE.Mailing.Domain.Configurations;
using Emeint.Core.BE.Mailing.Domain.Managers;
using Emeint.Core.BE.Mailing.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationManager = Emeint.Core.BE.Mailing.Domain.Configurations.ConfigurationManager;

namespace Emeint.Core.BE.Mailing
{
    public class MailingDependenciesResolver
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<SQLConnectionStringOptions>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();

            // Add application services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IEmailManager, SendGridEmailManager>();
            services.AddTransient<IHashingManager, HashingManager>();

            //services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddScoped<IMailRepository, MailRepository>();
            services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();

            services.AddScoped<WhiteListFilter>();

            services.AddScoped<SendingEmailQMessageConsumer>();

            InterCommDependenciesConfigurator.ConfigureServices(services, configuration, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.SendEmail", e => { e.Consumer<SendingEmailQMessageConsumer>(provider); });
            });
        }

    }
}
