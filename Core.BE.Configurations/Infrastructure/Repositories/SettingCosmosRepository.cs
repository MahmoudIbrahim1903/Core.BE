using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.Configurations.Application.Mappers;
using Emeint.Core.BE.Configurations.Domain.Enums;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Infrastructure.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Infrastructure.Repositories
{
    public class SettingCosmosRepository : BaseCosmosRepository<Setting>, ISettingRepository
    {
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;

        public SettingCosmosRepository(CosmosClient cosmosClient, IConfiguration configuration, ICacheService cacheService, IIdentityService identityService) : base(cosmosClient, configuration, "Settings", identityService)
        {
            _cacheService = cacheService;
            _configuration = configuration;
        }
        string CacheKey
        {
            get => _configuration["SettingCacheKeyName"];
        }

        public string GetSettingByKey(string key)
        {
            return GetAll().Where(s => s.Key == key).ToList()?.FirstOrDefault()?.Value;
        }

        public List<Setting> GetSettings(string key = null, string group = null, SettingUser? user = null, bool? showInPortal = null)
        {
            List<Setting> settings = (List<Setting>)_cacheService.GetItemFromCache<List<Setting>>(CacheKey); //GetListFromCache(CacheKey);

            if (settings == null || settings.Count == 0)
            {
                settings = GetAll().ToList();
                if (settings != null && settings.Count() > 0)
                    _cacheService.SaveItemToCache(CacheKey, settings.ToList()); //SaveToCache(CacheKey, settings.ToList());
            }

            if (!string.IsNullOrEmpty(key))
                settings = settings.Where(s => s.Key.Trim().ToLower() == key.Trim().ToLower()).ToList();
            if (!string.IsNullOrEmpty(group))
                settings = settings.Where(s => s.Group.Trim().ToLower() == group.Trim().ToLower()).ToList();
            if (user != null)
                settings = settings.Where(s => s.User == user || s.User == SettingUser.All).ToList();
            if (showInPortal != null)
                settings = settings.Where(s => s.ShowInPortal == showInPortal).ToList();
            return settings;
        }

        public bool RefreshCache()
        {
            var settings = GetAll();

            if (settings != null && settings.Count() > 0)
                _cacheService.SaveItemToCache(CacheKey, settings.ToList()); //SaveToCache(CacheKey, settings.ToList());

            return true;
        }

        public async Task<bool> RefreshCacheAsync()
        {
            return RefreshCache();
        }

        public async Task<bool> UpdateSettingAsync(string key, string value, string adminEmail)
        {
            var setting = GetAll().Where(s => s.Key == key).ToList()?.FirstOrDefault();
            if (setting != null)
            {
                setting.Validate(value.Trim());
                setting.Value = value;
                setting.ModificationDate = DateTime.UtcNow;
                setting.ModifiedBy = adminEmail;
                await UpdateAsync(setting);
                RefreshCache();
                return true;
            }
            return false;
        }
    }
}
