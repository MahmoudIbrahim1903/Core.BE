using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Emeint.Core.BE.InterCommunication.Concretes.AzureServiceBus
{
    public class ConsumerBase
    {
        private readonly ILogger<ConsumerBase> _logger;
        public ConsumerBase(string queueName, ILogger<ConsumerBase> logger)
        {
            QueueName = queueName;
            _logger = logger;
        }
        public string QueueName { set; get; }

        public T GetMessage<T>(Message message) where T : class
        {
            string msgString = Encoding.UTF8.GetString(message.Body);
            _logger.LogInformation($"msg to be consumed: {msgString}, msg type {typeof(T)}, queue name: {QueueName}");

            T qMessage = JsonSerializer.Deserialize<T>(msgString, new JsonSerializerOptions
            {
                IgnoreNullValues = true
            });
            return qMessage;
        }
    }
}
