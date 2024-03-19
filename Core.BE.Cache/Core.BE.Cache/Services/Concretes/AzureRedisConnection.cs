using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Cache.Services.Concretes
{
    public static class AzureRedisConnection
    {
        private static IConnectionMultiplexer _connection;
        public static IConnectionMultiplexer Connect(string connecttionString)
        {
            if (_connection == null)
                _connection = ConnectionMultiplexer.Connect(connecttionString);

            return _connection;
        }
    }
}
