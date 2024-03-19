using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Emeint.Core.BE.Domain.Models
{
    public class SQLConnectionStringOptions
    {
        public Dictionary<string, string> Connections { get; }
        protected string _connectionskey { get; set; } = "DbConnectionStrings";
        public SQLConnectionStringOptions(IConfiguration configuration)
        {
            Connections = new Dictionary<string, string>();
            GetConnectionStrings(configuration);

        }

        protected virtual void GetConnectionStrings(IConfiguration configuration)
        {

            configuration
                .GetSection(_connectionskey)
                .GetChildren()
                .ToList()
                .ForEach(section => Connections.Add(section.Key, section.Value));

        }

    }
}
