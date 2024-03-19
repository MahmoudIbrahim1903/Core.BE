using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Domain.Configurations
{
    public interface IConfigurationManager
    {
        bool EnableWhiteList();
        string GetWhiteList();
        string GetHtmlPath();
        string GetAssets();
        string GetSMTPSmtpIP();
        int GetSMTPSmtpPort();
        bool SMTPUseDefaultCredentials();
        string GetSMTPUserName();
        string GetSMTPPassword();
        string GetSMTPFrom();
        bool SMTPEnableSSL();
        string GetIntermicroservicesHashKey();
        int GetSMTPSmtpTimeout();
        string GetSendGridapiKey();
    }
}
