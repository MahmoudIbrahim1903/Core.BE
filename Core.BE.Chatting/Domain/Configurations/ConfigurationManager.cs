using Emeint.Core.BE.Configurations.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Domain.Configurations
{
    public class ConfigurationManager: IConfigurationManager
    {
        private readonly ISettingRepository _settingRepository;
        public ConfigurationManager(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }
        public string GetSendbirdToken()
        {
            return GetSettingByKey("SendbirdToken");
        }
        public string GetSendbirdAppId()
        {
            return GetSettingByKey("SendbirdAppId");
        }
        private string GetSettingByKey(string key)
        {
            return _settingRepository.GetSettingByKey(key);
        }

        public string GetChattingAdminId()
        {
            return GetSettingByKey("ChattingAdminId");
        }

        public string GetChattingAdminName()
        {
            return GetSettingByKey("ChattingAdminName");
        }
    }
}
