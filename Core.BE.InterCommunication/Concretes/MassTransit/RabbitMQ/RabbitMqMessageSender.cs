using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.InterCommunication.Contracts;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.RabbitMQ
{
    public class RabbitMqMessageSender : IBusMessageSender
    {
        private IBusControl _bus;
        private IIdentityService _identityService;
        private readonly RabbitMqConfigurations _rabbitMqConfigurations;



        public RabbitMqMessageSender(IBusControl bus, IIdentityService identityService, IConfiguration configuration)
        {
            _bus = bus;
            _identityService = identityService;
            _rabbitMqConfigurations = new RabbitMqConfigurations(configuration);
        }

        public async Task SendAndForget<T>(T obj, params string[] queues) where T : class
        {
            if (queues != null && queues.Length > 0)
            {
                //var URL = $"rabbitmq://localhost/grinta/";
                foreach (var queue in queues)
                {
                    var sendToUri = new Uri($"{_rabbitMqConfigurations.RabbitMqUri}{queue}");
                    var endPoint = await _bus.GetSendEndpoint(sendToUri);
                    await endPoint.Send<T>(obj, c => SetCountryHeader(c));
                }
            }
        }

        public async Task<TResponse> SendAndGetResponseAsync<TRequest, TResponse>(TRequest obj, string queue) where TResponse : class where TRequest : class
        {

            var requestTimeout = TimeSpan.FromSeconds(30);
            var sendToUri = new Uri($"{_rabbitMqConfigurations.RabbitMqUri}{queue}");
            var client = _bus.CreateRequestClient<TRequest>(sendToUri, timeout: requestTimeout);
            var response = await client.GetResponse<TResponse>(obj, callback: (x) => x.UseExecute(c => SetCountryHeader(c)), timeout: requestTimeout);
            return response.Message;
        }
        public async Task Publish<T>(T obj) where T : class
        {
            await _bus.Publish<T>(obj, c => SetCountryHeader(c));
        }

        private void SetCountryHeader(SendContext cc)
        {
            cc.Headers.Set("Country", _identityService.CountryCode);
        }

    }
}
