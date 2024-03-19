using Emeint.Core.BE.InterCommunication.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.Base
{
    public abstract class BusInterCommunicatorBase
    {

        private IBusMessageSender _messageSender;
        private string _queueName;

        public BusInterCommunicatorBase(IBusMessageSender messageSender, string destination)
        {
            _messageSender = messageSender;
            _queueName = destination;
        }

        public async Task SendAndForget<T>(T message) where T : class
        {
            await _messageSender.SendAndForget(message, _queueName);
        }
        
        public async Task<TResponse> SendAndGetResponse<TRequest, TResponse>(TRequest message) where TRequest : class where TResponse : class
        {
            var response = await _messageSender.SendAndGetResponseAsync<TRequest, TResponse>(message, _queueName);
            return response;
        }
    }
}
