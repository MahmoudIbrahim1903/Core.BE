using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Contracts.AzureServiceBus
{
    public interface IConsumer
    {
        string QueueName { get; set; }
        Task ConsumeAsync(Message message);
    }
}
