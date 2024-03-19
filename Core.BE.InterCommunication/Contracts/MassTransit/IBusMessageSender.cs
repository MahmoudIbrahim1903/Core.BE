using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Contracts
{
    public interface IBusMessageSender
    {
        Task SendAndForget<T>(T obj, params string[] queues) where T : class;
        Task<TResponse> SendAndGetResponseAsync<TRequest, TResponse>(TRequest obj, string queue) where TResponse : class where TRequest : class;
        Task Publish<T>(T obj) where T : class;
    }
}
