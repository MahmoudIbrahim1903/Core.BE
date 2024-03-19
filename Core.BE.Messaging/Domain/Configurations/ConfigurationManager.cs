using Emeint.Core.BE.Configurations.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Configurations
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ISettingRepository _settingRepository;
        public ConfigurationManager(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public bool EnableWhiteList()
        {
            return Convert.ToBoolean(GetSettingByKey("EnableWhiteList"));
        }

        public int GetMessageDelayValue()
        {
            return Convert.ToInt32(GetSettingByKey("MessageDelayValue"));
        }

        public string GetNotificationHubPath()
        {
            return GetSettingByKey("NotificationHubPath");
        }

        public string GetNotificationHubConnectionString()
        {
            return GetSettingByKey("NotificationHubConnectionString");
        }

        public string GetWhiteList()
        {
            return GetSettingByKey("WhiteList");
        }

        private string GetSettingByKey(string key)
        {
            return _settingRepository.GetSettingByKey(key)?.Trim() ?? string.Empty;
        }

        public string GetIntermicroservicesHashKey()
        {
            return GetSettingByKey("IntermicroservicesHashKey");
        }

        public string GetFirebaseMobileCredentialsFilePath()
        {
            return GetSettingByKey("FirebaseMobileCredentialsFilePath");
        }
        public string GetFirebaseWebCredentialsFilePath()
        {
            return GetSettingByKey("FirebaseWebCredentialsFilePath");
        }

        public bool GetTrackAdminNotifications()
        {
            return Convert.ToBoolean(GetSettingByKey("TrackAdminNotifications"));
        }

        public int GetNotificationsTrackingSLA()
        {
            var value = GetSettingByKey("NotificationTrackingSLAInDays");

            if (string.IsNullOrEmpty(value))
            {
                return 14;
            }

            return Convert.ToInt32(value);
        }
    }
}
