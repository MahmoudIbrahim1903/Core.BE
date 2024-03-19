using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Emeint.Core.BE.Identity.Domain.Configurations
{
    public class IdentityConfigurationManager : IIdentityConfigurationManager
    {
        private readonly ISettingRepository _settingRepository;
        public IdentityConfigurationManager(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        private string GetSettingByKey(string key)
        {
            return _settingRepository.GetSettingByKey(key)?.Trim() ?? string.Empty;
        }

        public PhoneNumberVerification GetAccountVerificationPhoneVerification()
        {
            return (PhoneNumberVerification)Convert.ToInt32(GetSettingByKey("AccountVerification:PhoneVerification").Trim());
        }

        public EmailVerification GetAccountVerificationEmailVerification()
        {
            return (EmailVerification)Convert.ToInt32(GetSettingByKey("AccountVerification:EmailVerification").Trim());
        }

        public string GetDefaultCountry()
        {
            var value = GetSettingByKey("DefaultCountry");
            if (string.IsNullOrEmpty(value))
                value = "EGY";
            return value;
        }

        public int GetMaxFailedAccessAttempts()
        {
            var value = GetSettingByKey("MaxFailedAccessAttempts");
            if (string.IsNullOrEmpty(value))
                value = "3";
            return Convert.ToInt32(value);
        }

        public int GetDefaultLockoutMinutes()
        {
            var value = GetSettingByKey("DefaultLockoutMinutes");
            if (string.IsNullOrEmpty(value))
                value = "5";
            return Convert.ToInt32(value);
        }

        public string GetEmailFrom()
        {
            //returned from business configuration
            return GetSettingByKey("EmailFrom");
        }

        public string GetCreateAdminEmailSubject()
        {
            return GetSettingByKey("CreateAdminEmailSubject");
        }

        public string GetUpdateAdminEmailSubject()
        {
            return GetSettingByKey("UpdateAdminEmailSubject");
        }

        public string GetServerBaseUrl()
        {
            return GetSettingByKey("ServerBaseUrl");
        }
        //public int GetSendEmailBlockIntervalByMinutes()
        //{
        //    var value = GetSettingByKey("SendEmailBlockIntervalByMinutes");
        //    if (string.IsNullOrEmpty(value))
        //        value = "5";
        //    return Convert.ToInt32(value);
        //}

        //public int GetSendSmsBlockIntervalByMinutes()
        //{
        //    var value = GetSettingByKey("SendSmsBlockIntervalByMinutes");
        //    if (string.IsNullOrEmpty(value))
        //        value = "5";
        //    return Convert.ToInt32(value);
        //}

        public bool IsConcurrentSessionsAllowed()
        {
            var value = GetSettingByKey("IsConcurrentSessionsAllowed");
            if (string.IsNullOrEmpty(value))
                value = "true";
            return bool.Parse(value);
        }
        public string GetResetPasswordSubject()
        {
            return GetSettingByKey("ResetPasswordSubject");
        }
        //public string GetSendAdminResetPasswordOTPSubject()
        //{
        //    return GetSettingByKey("SendAdminResetPasswordOTPSubject");
        //}
    }
}
