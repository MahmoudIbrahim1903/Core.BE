using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.Extensions.Configuration;
using System;

namespace Emeint.Core.BE.Mailing.Domain.Configurations
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
        private string GetSettingByKey(string key)
        {

            return _settingRepository.GetSettingByKey(key.Trim())?.Trim() ?? string.Empty;
        }

        public bool EnableWhiteList()
        {
            return Convert.ToBoolean(GetSettingByKey("EnableWhiteList"));
        }

        public string GetAssets()
        {
            return GetSettingByKey("Assets");
        }

        public string GetHtmlPath()
        {
            return GetSettingByKey("HtmlPath");
        }

        public string GetSMTPFrom()
        {
            return GetSettingByKey("SMTP:From");
        }

        public string GetSMTPPassword()
        {
            return GetSettingByKey("SMTP:Password");
        }

        public string GetSMTPSmtpIP()
        {
            return GetSettingByKey("SMTP:SmtpIP");
        }

        public int GetSMTPSmtpPort()
        {
            return Convert.ToInt32(GetSettingByKey("SMTP:SmtpPort"));
        }

        public int GetSMTPSmtpTimeout()
        {
            return Convert.ToInt32(GetSettingByKey("SMTP:SmtpTimeout"));
        }

        public string GetSMTPUserName()
        {
            return GetSettingByKey("SMTP:UserName");
        }

        public string GetWhiteList()
        {
            return GetSettingByKey("WhiteList");
        }

        public bool SMTPEnableSSL()
        {
            return Convert.ToBoolean(GetSettingByKey("SMTP:EnableSSL"));
        }

        public bool SMTPUseDefaultCredentials()
        {
            return Convert.ToBoolean(GetSettingByKey("SMTP:UseDefaultCredentials"));
        }

        public string GetSendGridapiKey()
        {
            var readAppSettingsFromKeyVault = _configuration.GetValue<bool>("IsKeyVaultEnabled", false);
            if (!readAppSettingsFromKeyVault)
            {
                return GetSettingByKey("SendGrid:apiKey");
            }

            return _configuration.GetValue<string>("SendGrid:APIKey");

        }

        public string GetIntermicroservicesHashKey()
        {
            return GetSettingByKey("IntermicroservicesHashKey");
        }
    }
}
