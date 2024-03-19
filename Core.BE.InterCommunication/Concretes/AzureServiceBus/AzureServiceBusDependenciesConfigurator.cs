using Emeint.Core.BE.InterCommunication.Contracts.AzureServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.AzureServiceBus
{
    public static class AzureServiceBusDependenciesConfigurator
    {
        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var services = serviceCollection.BuildServiceProvider();
            var queueClients = services.GetService<IEnumerable<IQueueClient>>();
            var consumers = services.GetService<IEnumerable<IConsumer>>();
            var loggerFactory = services.GetService<ILoggerFactory>();
            var configuration = services.GetService<IConfiguration>();

            //configure logger service
            int retainedLogFileCountLimit = configuration.GetValue<int>("RetainedLogFileCountLimit");
            if (retainedLogFileCountLimit > 0)
            {
                loggerFactory.AddFile(configuration["Logging:FileName"], retainedFileCountLimit: retainedLogFileCountLimit);
            }
            else
            {
                loggerFactory.AddFile(configuration["Logging:FileName"]);
            }

            var logger = services.GetService<ILogger<ConsumerBase>>();

            if (queueClients != null && queueClients.Count() > 0 && consumers != null && consumers.Count() > 0)
            {
                foreach (var queueClient in queueClients)
                {
                    var consumer = consumers.FirstOrDefault(c => c.QueueName == queueClient.QueueName);

                    if (consumer != null)
                    {
                        queueClient.RegisterMessageHandler(
                        async (msg, cancellationToken) =>
                        {
                            //resolve a scoped instance of consumer
                            using (var scope = services.CreateScope())
                            {
                                consumers = scope.ServiceProvider.GetService<IEnumerable<IConsumer>>();
                                consumer = consumers.FirstOrDefault(c => c.QueueName == queueClient.QueueName);
                                await consumer.ConsumeAsync(msg);
                            }

                        },
                    (exHandler) =>
                    {
                        logger.LogInformation($"exception while consuming message:{exHandler.Exception.Message}, inner exception:{exHandler.Exception.InnerException}, stack trace: {exHandler.Exception.StackTrace}");
                        return Task.CompletedTask;
                    });
                    }
                }
            }

        }
    }
}
