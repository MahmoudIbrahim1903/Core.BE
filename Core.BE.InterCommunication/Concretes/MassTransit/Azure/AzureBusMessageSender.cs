using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.InterCommunication.Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.Azure
{
    public class AzureBusMessageSender : IBusMessageSender
    {
        private IBusControl _bus;
        private IIdentityService _identityService;
        private readonly AzureBusConfigurations _azureBusConfigurations;


        public AzureBusMessageSender(IBusControl bus, IIdentityService identityService, IConfiguration configuration)
        {
            _bus = bus;
            _identityService = identityService;
            _azureBusConfigurations = new AzureBusConfigurations(configuration);
        }

        public async Task Publish<T>(T obj) where T : class
        {
            await _bus.Publish<T>(obj, c => SetCountryHeader(c));
        }

        public async Task SendAndForget<T>(T obj, params string[] queues) where T : class
        {
            if (queues != null && queues.Length > 0)
            {
                foreach (var queue in queues)
                {
                    var sendToUri = new Uri($"sb://{_azureBusConfigurations.AzureSbNamespace}.servicebus.windows.net/{queue}");
                    var endPoint = await _bus.GetSendEndpoint(sendToUri);
                    await endPoint.Send<T>(obj, c => SetCountryHeader(c));
                }
            }
        }

        public async Task<TResponse> SendAndGetResponseAsync<TRequest, TResponse>(TRequest obj, string queue)
            where TRequest : class
            where TResponse : class
        {

            throw new Exception("Operation is not supported by vendor.");

            //var requestTimeout = TimeSpan.FromSeconds(30);
            //var sendToUri = new Uri($"sb://{_azureBusConfigurations.AzureSbNamespace}.servicebus.windows.net/{queue}");

            //var client = new MessageRequestClient<TRequest, TResponse>(_bus, sendToUri, requestTimeout);
            //var res = await client.Request(obj);
            //return res;
        }
        private void SetCountryHeader(SendContext cc)
        {
            cc.Headers.Set("Country", _identityService.CountryCode);
        }

    }
}