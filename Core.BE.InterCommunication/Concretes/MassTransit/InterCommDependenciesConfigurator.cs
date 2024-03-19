using Emeint.Core.BE.InterCommunication.Concretes;
//using MassTransit.AzureServiceBusTransport;
using Emeint.Core.BE.InterCommunication.Concretes.Azure;
using Emeint.Core.BE.InterCommunication.Concretes.Notifier;
using Emeint.Core.BE.InterCommunication.Concretes.RabbitMQ;
using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Contracts.Notifier;
using MassTransit;
using MassTransit.Scoping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Emeint.Core.BE.InterCommunication
{
    public static class InterCommDependenciesConfigurator
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration,
            Action<IServiceProvider, IBusFactoryConfigurator> registrationAction = null)
        {

            services.AddScoped<ScopedConsumeContextProvider>();
            var serviceBusImplementation = configuration.GetValue<string>("ServiceBusImplementation", string.Empty);
            var isAzureImplementation = serviceBusImplementation.ToLower().Equals("azure");
            services.AddSingleton(provider =>
            {

                return isAzureImplementation ?
                                        AzureBusConfigurator.ConfigureBusWithAzure(provider, configuration, registrationAction) :
                                        RabbitMqBusConfigurator.ConfigureBusWithRabbitMq(provider, configuration, registrationAction);
            });

            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            if (isAzureImplementation)
                services.AddScoped<IBusMessageSender, AzureBusMessageSender>();
            else
                services.AddScoped<IBusMessageSender, RabbitMqMessageSender>();


            services.AddHostedService<BusBackgroundService>();

            #region Register Services
            services.AddScoped<ISendPushNotifierService, SendPushNotifierService>();
            services.AddScoped<ISendSmsNotifierService, SendSmsNotifierService>();
            #endregion
        }
    }
}
