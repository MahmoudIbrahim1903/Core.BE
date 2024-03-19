using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Contracts.AzureServiceBus
{
    public interface INotifierService<T> where T : class
    {
        Task NotifyAsync(T message, Dictionary<string, string> customProperties, string queueName, string messageId);
    }
}
