using Emeint.Core.BE.Configurations.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Domain.Model
{
    public interface ISettingRepository : IAsyncRepository<Setting>
    {
        List<Setting> GetSettings(string key = null, string group = null, SettingUser? user = null, bool? showInPortal = null);
        //Task<List<Setting>> GetSettingsAsync(string key = null, string group = null, SettingUser? user = null, bool? showInPortal = null);
        string GetSettingByKey(string key);
        //Task<string> GetSettingByKeyAsync(string key);
        Task<bool> RefreshCacheAsync();
        bool RefreshCache();
        Task<bool> UpdateSettingAsync(string key, string value, string adminEmail);

    }
}
