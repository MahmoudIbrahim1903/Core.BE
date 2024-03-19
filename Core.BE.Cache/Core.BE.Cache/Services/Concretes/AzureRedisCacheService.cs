
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace Emeint.Core.BE.Cache.Services.Concretes
{
    public class AzureRedisCacheService : ICacheService
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabase _azureRedisCache;
        private static IConnectionMultiplexer _connection;
        private readonly ILogger<AzureRedisCacheService> _logger;
        private readonly string _countryCode;

        public AzureRedisCacheService(IConfiguration configuration, ILogger<AzureRedisCacheService> logger, IIdentityService identityService)
        {
            _configuration = configuration;
            _connection = AzureRedisConnection.Connect(_configuration["AzureRedisCacheConnectionString"]);
            _azureRedisCache = _connection.GetDatabase();
            _logger = logger;
            _countryCode = (!string.IsNullOrEmpty(identityService.CountryCode) ? identityService.CountryCode : "EGY") + "_";

        }

        public void ClearCacheRegion(string regionKey)
        {
            //clearing all cache as region is not supported
            ResetCache();
        }

        public bool DeleteItemFromCache(string itemKey)
        {
            return _azureRedisCache.KeyDelete(_countryCode + itemKey);
        }

        public bool DeleteItemFromCacheRegion(string regionKey, string itemKey)
        {
            //region not supported
            return _azureRedisCache.KeyDelete(regionKey + "_" + _countryCode + itemKey);
        }
        public T GetItemFromCache<T>(string itemKey)
        {
            if (_configuration["CachingEnabled"] == false.ToString())
                return default(T);
            try
            {
                var redisItem = _azureRedisCache.StringGet(_countryCode + itemKey);
                if (redisItem.HasValue)
                {
                    var cachedItem = JsonConvert.DeserializeObject<T>(redisItem);
                    return cachedItem;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while getting setting from redis server for item:{itemKey}, exception: {ex.Message}");
            }
            return default(T);
        }
        public T GetItemFromCacheRegion<T>(string regionKey, string itemKey)
        {
            if (_configuration["CachingEnabled"] == false.ToString())
                return default(T);
            try
            {
                var redisItem = _azureRedisCache.StringGet(regionKey + "_" + _countryCode + itemKey);
                if (redisItem.HasValue)
                {
                    var cachedItem = JsonConvert.DeserializeObject<T>(redisItem);
                    return cachedItem;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while getting setting from redis server for item:{itemKey}, exception: {ex.Message}");
            }
            return default(T);
        }

        public void ResetCache()
        {
            var endpoints = _connection.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        public bool SaveItemToCache<T>(string itemKey, T value)
        {
            try
            {
                return _azureRedisCache.StringSet(_countryCode + itemKey, JsonConvert.SerializeObject(value));
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while saving setting to redis server for item:{_countryCode + itemKey}, exception: {ex.Message}");
            }
            return false;
        }
        public bool SaveItemToCache<T>(string itemKey, T value, TimeSpan absoluteExpiration)
        {
            try
            {
                return _azureRedisCache.StringSet(itemKey, JsonConvert.SerializeObject(value), absoluteExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while saving setting to redis server for item:{itemKey}, exception: {ex.Message}");
            }
            return false;
        }
        public bool SaveItemToCacheRegion<T>(string regionKey, string itemKey, T value)
        {
            try
            {
                return _azureRedisCache.StringSet(regionKey + "_" + _countryCode + itemKey, JsonConvert.SerializeObject(value, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while saving setting to redis server for item:{itemKey}, exception: {ex.Message}");
            }
            return false;
        }
        public bool SaveItemToCacheRegion<T>(string regionKey, string itemKey, T value, TimeSpan absoluteExpiration)
        {
            try
            {
                return _azureRedisCache.StringSet(regionKey + "_" + _countryCode + itemKey, JsonConvert.SerializeObject(value, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }), absoluteExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while saving setting to redis server for item:{itemKey}, exception: {ex.Message}");
            }
            return false;
        }
    }
}