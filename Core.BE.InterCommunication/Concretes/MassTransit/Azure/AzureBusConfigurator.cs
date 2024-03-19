using MassTransit;
//using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
//using MassTransit.AzureServiceBusTransport;
//using MassTransit.AzureServiceBusTransport.Configuration;
//using Microsoft.ServiceBus;
using System;

namespace Emeint.Core.BE.InterCommunication.Concretes.Azure
{
    public static class AzureBusConfigurator
    {
        public static IBusControl ConfigureBusWithAzure(IServiceProvider serviceProvider, IConfiguration configuration, Action<IServiceProvider, IBusFactoryConfigurator> registrationAction = null)
        {
            var azureBusConfigurations = new AzureBusConfigurations(configuration);
            return Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                var serviceUri = new Uri($"sb://{azureBusConfigurations.AzureSbNamespace}.servicebus.windows.net/");
                cfg.Host(serviceUri, h =>
                {
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(
                        azureBusConfigurations.AzureSbKeyName, azureBusConfigurations.AzureSbSharedAccessKey, TimeSpan.FromDays(1), TokenScope.Namespace);
                });

                //.. You can also make some custom configuration using the configuration object ..//
                //cfg.PrefetchCount = 1;
                //cfg.PurgeOnStartup = true;
                //cfg.AutoDelete = false;
                //cfg.Durable = false;
                //cfg.Exclusive = false;
                //cfg.ExchangeType = ExchangeType.Direct;

                registrationAction?.Invoke(serviceProvider, cfg);
            });
        }
    }
}
