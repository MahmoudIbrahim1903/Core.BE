using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.InterCommunication.Concretes
{
    public class BusConfigurations
    {
        protected readonly IConfiguration Configuration;

        public BusConfigurations(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string SubscribedQueue
        {
            get
            {
                return Configuration["SubscribedQueue"];
            }
        }

        public string ServiceBusImplementation
        {
            get
            {
                return Configuration["ServiceBusImplementation"];
            }
        }

        public string GetValue(string key)
        {
            return Configuration[key];
        }
    }
}
