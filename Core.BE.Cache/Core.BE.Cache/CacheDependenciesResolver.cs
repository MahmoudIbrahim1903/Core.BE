using CacheManager.Core;
using Emeint.Core.BE.Cache.Services.Concretes;
using Emeint.Core.BE.Cache.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Cache
{
    public static class CacheDependenciesResolver
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            if (configuration["CacheType"] == "Redis")
            {
                services.AddScoped(typeof(ICacheService), typeof(AzureRedisCacheService));
            }
            else
            {
                services.AddSingleton(typeof(ICacheManagerConfiguration),
                      new CacheManager.Core.ConfigurationBuilder()
                     .WithJsonSerializer()
                     .WithMicrosoftMemoryCacheHandle()
                     .Build());

                services.AddSingleton(typeof(ICache<>), typeof(BaseCacheManager<>));
                services.AddScoped(typeof(ICacheService), typeof(MemoryCacheService));
            }
        }

    }
}
