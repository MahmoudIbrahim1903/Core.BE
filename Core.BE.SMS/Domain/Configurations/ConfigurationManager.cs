using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.SMS.Domain.Configurations
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IConfiguration _configuration;

        public ConfigurationManager(ISettingRepository settingRepository, IConfiguration configuration)
        {
            _settingRepository = settingRepository;
            _configuration = configuration;
        }

        public string GetVictoryLinkSMSPassword()
        {
            var readAppSettingsFromKeyVault = _configuration.GetValue<bool>("IsKeyVaultEnabled", false);
            if (!readAppSettingsFromKeyVault)
            {
                return GetSettingByKey("VictoryLinkSMSPassword");
            }

            return _configuration.GetValue<string>("VictoryLinkSMSPassword");
        }

        public string GetVictoryLinkSMSSender()
        {
            return GetSettingByKey("VictoryLinkSMSSender");
        }

        public string GetEZagelSMSSender()
        {
            return GetSettingByKey("EZagelSMSSender");
        }

        public string GetVictoryLinkSMSUserName()
        {
            var readAppSettingsFromKeyVault = _configuration.GetValue<bool>("IsKeyVaultEnabled", false);
            if (!readAppSettingsFromKeyVault)
            {
                return GetSettingByKey("VictoryLinkSMSUserName");
            }

            return _configuration.GetValue<string>("VictoryLinkSMSUserName");
        }

        public string GetTwilioAccountSId()
        {
            return GetSettingByKey("TwilioAccountSId");
        }

        public string GetTwilioAuthToken()
        {
            return GetSettingByKey("TwilioAuthToken");
        }

        public string GetTwilioFromPhoneNumber()
        {
            return GetSettingByKey("TwilioFromPhoneNumber");
        }

        private string GetSettingByKey(string key)
        {
            return _settingRepository.GetSettingByKey(key);
        }

        public string GetEzagelSMSUserName()
        {
            return GetSettingByKey("EzagelSMSUserName");
        }

        public string GetEzagelSMSPassword()
        {
            return GetSettingByKey("EzagelSMSPassword");
        }
        public string GetIntermicroservicesHashKey()
        {
            return GetSettingByKey("IntermicroservicesHashKey");
        }

        public string GetArpuUserName()
        {
            return GetSettingByKey("ArpuUserName");
        }

        public string GetArpuPassword()
        {
            return GetSettingByKey("ArpuPassword");
        }

        public string GetArpuAccountId()
        {
            return GetSettingByKey("ArpuAccountId");
        }

        public string GetArpuSingleSmsUrl()
        {
            return GetSettingByKey("ArpuUrl");
        }

        public string GetArpuSMSSender()
        {
            return GetSettingByKey("ArpuSMSSender");
        }

        public string GetCequensToken()
        {
            var readAppSettingsFromKeyVault = _configuration.GetValue<bool>("IsKeyVaultEnabled", false);
            if (!readAppSettingsFromKeyVault)
            {
                return GetSettingByKey("CequensToken");
            }

            return _configuration.GetValue<string>("CequensToken");
        }

        public string GetCequensSendSmsUrl()
        {
            return GetSettingByKey("CequensSendSmsUrl");
        }

        public string GetCequensSenderName()
        {
            return GetSettingByKey("CequensSenderName");
        }
    }
}
