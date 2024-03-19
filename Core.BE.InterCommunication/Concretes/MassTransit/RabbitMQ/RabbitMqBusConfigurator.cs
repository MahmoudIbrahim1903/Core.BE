using Emeint.Core.BE.API.Infrastructure.Services;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using System;

namespace Emeint.Core.BE.InterCommunication.Concretes.RabbitMQ
{
    public static class RabbitMqBusConfigurator
    {
        public static IBusControl ConfigureBusWithRabbitMq(IServiceProvider serviceProvider, IConfiguration configuration,
            Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> registrationAction = null)
        {
            var rabbitMqConfigurations = new RabbitMqConfigurations(configuration);
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri(rabbitMqConfigurations.RabbitMqUri), hst =>
                {
                    hst.Username(rabbitMqConfigurations.RabbitMqUserName);
                    hst.Password(rabbitMqConfigurations.RabbitMqPassword);
                });

                var identityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

                //cfg.ConfigureSend(s => s.UseSendExecute(context => context.Headers.Set("Country", identityService.CountryCode)));

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
