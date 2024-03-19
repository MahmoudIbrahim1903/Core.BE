using CacheManager.Core;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Cache.Services.Concretes
{
    public class MemoryCacheService : ICacheService
    {
        private ICache<object> _cacheRepository;
        private readonly IConfiguration _configuration;
        private readonly string _countryCode;

        public MemoryCacheService(ICache<object> cacheRepository, IConfiguration configuration, IIdentityService identityService)
        {
            _cacheRepository = cacheRepository;
            _configuration = configuration;
            _countryCode = (!string.IsNullOrEmpty(identityService.CountryCode) ? identityService.CountryCode : "EGY") + "_";
        }

        public void ClearCacheRegion(string regionKey)
        {
            _cacheRepository.ClearRegion(regionKey);
        }

        public bool DeleteItemFromCache(string itemKey)
        {
            var res = _cacheRepository.Remove(_countryCode + itemKey);
            return res;
        }

        public bool DeleteItemFromCacheRegion(string regionKey, string itemKey)
        {
            var res = _cacheRepository.Remove(_countryCode + itemKey, regionKey);
            return res;
        }

        public T GetItemFromCache<T>(string itemKey)
        {
            if (_configuration["CachingEnabled"] == false.ToString())
                return default(T);

            return (T)_cacheRepository.Get(_countryCode + itemKey);
        }

        public T GetItemFromCacheRegion<T>(string regionKey, string itemKey)
        {
            if (_configuration["CachingEnabled"] == false.ToString())
                return default(T);

            return (T)_cacheRepository.Get(_countryCode + itemKey, regionKey);
        }

        public void ResetCache()
        {
            _cacheRepository.Clear();
        }

        public bool SaveItemToCache<T>(string itemKey, T value)
        {
            if (_cacheRepository.Exists(_countryCode + itemKey))
                _cacheRepository.Put(_countryCode + itemKey, value);
            else
            {
                var res = _cacheRepository.Add(_countryCode + itemKey, value);
                return res;
            }
            return true;
        }

        public bool SaveItemToCache<T>(string itemKey, T value, TimeSpan absoluteExpiration)
        {
            if (_cacheRepository.Exists(_countryCode + itemKey))
            {
                var cacheItem = _cacheRepository.GetCacheItem(_countryCode + itemKey);
                _cacheRepository.Put(cacheItem.WithAbsoluteExpiration(absoluteExpiration));
            }
            else
            {
                var item = new CacheItem<object>(_countryCode + itemKey, value, ExpirationMode.Absolute, absoluteExpiration);
                var res = _cacheRepository.Add(item);
                return res;
            }
            return true;
        }

        public bool SaveItemToCacheRegion<T>(string regionKey, string itemKey, T value)
        {
            if (_configuration["CachingEnabled"] == false.ToString())
                return false;

            if (_cacheRepository.Exists(_countryCode + itemKey, regionKey))
                _cacheRepository.Put(_countryCode + itemKey, value, regionKey);
            else
            {
                var res = _cacheRepository.Add(_countryCode + itemKey, value, regionKey);
                return res;
            }
            return true;
        }

        public bool SaveItemToCacheRegion<T>(string regionKey, string itemKey, T value, TimeSpan absoluteExpiration)
        {
            if (_configuration["CachingEnabled"] == false.ToString())
                return false;

            if (_cacheRepository.Exists(_countryCode + itemKey, regionKey))
            {
                var cacheItem = _cacheRepository.GetCacheItem(_countryCode + itemKey, regionKey);
                _cacheRepository.Put(cacheItem.WithAbsoluteExpiration(absoluteExpiration));
            }
            else
            {
                var item = new CacheItem<object>(_countryCode + itemKey, regionKey, value, ExpirationMode.Absolute, absoluteExpiration);
                var res = _cacheRepository.Add(item);
                return res;
            }
            return true;
        }
    }
}
