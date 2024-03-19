using Emeint.Core.BE.Configurations.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Configurations
{
    public interface IConfigurationManager
    {
        string GetVictoryLinkSMSUserName();
        string GetVictoryLinkSMSPassword();
        string GetVictoryLinkSMSSender();
        string GetTwilioAccountSId();
        string GetTwilioAuthToken();
        string GetTwilioFromPhoneNumber();
        string GetIntermicroservicesHashKey();
        string GetEzagelSMSUserName();
        string GetEzagelSMSPassword();
        string GetEZagelSMSSender();
        string GetArpuUserName();
        string GetArpuPassword();
        string GetArpuAccountId();
        string GetArpuSingleSmsUrl();
        string GetArpuSMSSender();
        string GetCequensToken();
        string GetCequensSendSmsUrl();
        string GetCequensSenderName();
    }
}
