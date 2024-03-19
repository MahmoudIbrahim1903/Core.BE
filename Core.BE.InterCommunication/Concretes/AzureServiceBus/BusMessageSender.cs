using Emeint.Core.BE.InterCommunication.Contracts.AzureServiceBus;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.InterCommunication.Concretes.AzureServiceBus
{
    public class BusMessageSender : IBusMessageSender
    {
        private readonly IEnumerable<IQueueClient> _queueClients;

        public BusMessageSender(IEnumerable<IQueueClient> queueClients)
        {
            _queueClients = queueClients;
        }

        public Task PublishAsync<T>(T obj, Dictionary<string, string> customProperties, string queueName) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task SendAndForgetAsync<T>(T obj, Dictionary<string, string> customProperties, string queueName, string messageId) where T : class
        {
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));

            message.MessageId = messageId ?? Guid.NewGuid().ToString();
            message.CorrelationId = "";
            message.ContentType = "application/json";

            if (customProperties != null && customProperties.Any())
                foreach (var prop in customProperties)
                    message.UserProperties.Add(prop.Key, prop.Value);


            var queue = _queueClients.FirstOrDefault(c => c.QueueName == queueName);
            await queue.SendAsync(message);
        }
    }
}
