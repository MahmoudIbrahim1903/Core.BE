using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.InterCommunication.Concretes.RabbitMQ
{
    public class RabbitMqConfigurations : BusConfigurations
    {
        public RabbitMqConfigurations(IConfiguration configuration) : base(configuration)
        {
        }

        public string RabbitMqUri
        {
            get
            {
                return $"rabbitmq://{RabbitMqHostname}/{RabbitMqVirtualHost}/";
            }

        }

        public string RabbitMqVirtualHost
        {
            get
            {
                return Configuration["RabbitMqVirtualHost"];
            }
        }

        public string RabbitMqHostname
        {
            get
            {
                return Configuration["RabbitMqHostname"];
            }
        }

        public string RabbitMqUserName
        {
            get
            {
                return Configuration["RabbitMqUserName"];
            }
        }

        public string RabbitMqPassword
        {
            get
            {
                return Configuration["RabbitMqPassword"];
            }
        }
    }
}
