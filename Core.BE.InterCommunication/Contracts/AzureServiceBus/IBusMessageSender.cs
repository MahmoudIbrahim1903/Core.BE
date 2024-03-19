using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Contracts.AzureServiceBus
{
    public interface IBusMessageSender
    {
        Task SendAndForgetAsync<T>(T obj, Dictionary<string, string> customProperties, string queue, string messageId) where T : class;

        public Task PublishAsync<T>(T obj, Dictionary<string, string> customProperties, string queueName) where T : class;
    }
}
