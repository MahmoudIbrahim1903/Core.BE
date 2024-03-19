using Emeint.Core.BE.InterCommunication.Contracts.AzureServiceBus;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.AzureServiceBus
{
    public class NotifierServiceBase<T> : BusMessageSender where T : class
    {
        public NotifierServiceBase(IEnumerable<IQueueClient> queueClients) : base(queueClients)
        {

        }
        public async Task NotifyAsync(T message, Dictionary<string, string> customProperties, string queueName, string messageId)
        {
            await SendAndForgetAsync(message, customProperties, queueName, messageId);
        }
    }
}

