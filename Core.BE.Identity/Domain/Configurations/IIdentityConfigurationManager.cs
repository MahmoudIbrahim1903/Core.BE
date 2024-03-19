using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Configurations
{
    public interface IIdentityConfigurationManager
    {

        PhoneNumberVerification GetAccountVerificationPhoneVerification();
        EmailVerification GetAccountVerificationEmailVerification();
        string GetDefaultCountry();
        int GetMaxFailedAccessAttempts();
        int GetDefaultLockoutMinutes();
        string GetEmailFrom();
        string GetCreateAdminEmailSubject();
        string GetUpdateAdminEmailSubject();
        string GetServerBaseUrl();
        //int GetSendEmailBlockIntervalByMinutes();
        //int GetSendSmsBlockIntervalByMinutes();
        public bool IsConcurrentSessionsAllowed();
        string GetResetPasswordSubject();
        //string GetSendAdminResetPasswordOTPSubject();
    }
}
