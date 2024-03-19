using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.Configurations.Application.Mappers;
using Emeint.Core.BE.Configurations.Domain.Enums;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Infrastructure.Repositories
{
    public class SettingRepository
       : BaseRepository<Setting, ConfigurationContext>, ISettingRepository
    {
        private ICacheService _cacheService;
        private readonly IConfiguration _configuration;
        public SettingRepository(ConfigurationContext context, ICacheService cacheService, IConfiguration configuration) : base(context)
        {
            _cacheService = cacheService;
            _configuration = configuration;
        }

        //void SaveToCache(string cachKey, List<Setting> settings)
        //{
        //    _memoryCache.Set(cachKey, settings);
        //}
        //void SaveToCache(string cachKey, string settingValue)
        //{
        //    _memoryCache.Set(cachKey, settingValue);
        //}
        //List<Setting> GetListFromCache(string cachKey)
        //{
        //    _memoryCache.TryGetValue(cachKey, out List<Setting> settings);
        //    return settings;
        //}
        //string GetValueFromCache(string cachKey)
        //{
        //    _memoryCache.TryGetValue(cachKey, out string setting);
        //    return setting;
        //}


        string CacheKey
        {
            get => _configuration["SettingCacheKeyName"];
        }

        public List<Setting> GetSettings(string key = null, string group = null, SettingUser? user = null, bool? showInPortal = null)
        {
            List<Setting> settings = (List<Setting>)_cacheService.GetItemFromCache<List<Setting>>(CacheKey); //GetListFromCache(CacheKey);

            if (settings == null || settings.Count == 0)
            {
                settings = _context.Settings.ToList();
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
        public string GetSettingByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return String.Empty;

            var settings = GetSettings(key);
            var settingValue = settings.FirstOrDefault(s => s.Key.ToLower().Trim() == key.ToLower().Trim())?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(settingValue))
                settingValue = _context.Settings.FirstOrDefault(s => s.Key.Trim().ToLower() == key.Trim().ToLower())?.Value ?? string.Empty;
            return settingValue;
        }
        //public async Task<string> GetSettingByKeyAsync(string key)
        //{
        //    return await Task.Run(() => GetSettingByKey(key));
        //}
        //public async Task<List<Setting>> GetSettingsAsync(string key = null, string group = null, SettingUser? user = null, bool? showInProtal = null)
        //{
        //    return await Task.Run(() => GetSettings(key, group, user, showInProtal));
        //}

        public bool RefreshCache()
        {
            var settings = _context.Settings.ToList();

            if (settings != null && settings.Count() > 0)
                _cacheService.SaveItemToCache(CacheKey, settings.ToList()); //SaveToCache(CacheKey, settings.ToList());

            return true;
        }
        public async Task<bool> RefreshCacheAsync()
        {
            return await Task.Run(() => RefreshCache());
        }

        public async Task<bool> UpdateSettingAsync(string key, string value, string adminEmail)
        {
            value = value?.Trim();
            var setting = _context.Settings.FirstOrDefault(s => s.Key == key);
            if (setting != null)
            {
                setting.Validate(value);
                setting.Value = value;
                setting.ModificationDate = DateTime.UtcNow;
                setting.ModifiedBy = adminEmail;
                await _context.SaveChangesAsync();
                RefreshCache();
                return true;
            }
            return false;

        }

        async Task<Setting> IAsyncRepository<Setting>.AddAsync(Setting entity)
        {
            return base.Add(entity);
        }

        async Task IAsyncRepository<Setting>.UpdateAsync(Setting entity)
        {
            base.Update(entity);
        }

        async Task IAsyncRepository<Setting>.DeleteAsync(Setting entity)
        {
            base.Delete(entity);
        }
    }
}
