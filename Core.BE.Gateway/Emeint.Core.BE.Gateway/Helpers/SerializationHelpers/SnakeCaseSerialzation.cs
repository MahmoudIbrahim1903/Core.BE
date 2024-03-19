using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Helpers.SerializationHelpers
{
    public class SnakeCaseSerialzation
    {
        public DefaultContractResolver SerialzeResolver { get; set; }
        public SnakeCaseSerialzation()
        {
            SerialzeResolver = contractResolver;
        }

        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
    }
}
