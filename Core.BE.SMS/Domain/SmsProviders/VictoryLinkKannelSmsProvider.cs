using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VictoryLinkServiceReference;

namespace Emeint.Core.BE.SMS.Domain.SmsProviders
{
    public class VictoryLinkKannelSmsProvider : ISmsSender
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IIdentityService _identityService;
        public VictoryLinkKannelSmsProvider(IConfigurationManager configurationManager, IIdentityService identityService)
        {
            _configurationManager = configurationManager;
            _identityService = identityService;
        }

        public Task<int> SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            var smsUserName = _configurationManager.GetVictoryLinkSMSUserName();
            var smsPassword = _configurationManager.GetVictoryLinkSMSPassword();
            var smsSender = _configurationManager.GetVictoryLinkSMSSender();
            var language = _identityService.Language == Language.ar ? "A" : "E";

            var client = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
            //var result = client.SendSMSAsync(smsUserName, smsPassword, message, language, smsSender, number).Result;
            var result = client.SendSMSWithDLRAsync(smsUserName, smsPassword, message, language, smsSender, number).Result;
            switch (result)
            {
                case -1:
                    throw new SendSmsFailedException(number, "User is not subscribed.", -1);
                case -5:
                    throw new SendSmsFailedException(number, "out of credit.", -5);
                case -10:
                    throw new SendSmsFailedException(number, "Queued Message, no need to send it again.", -10);
                case -11:
                    throw new SendSmsFailedException(number, "Invalid language.", -11);
                case -12:
                    throw new SendSmsFailedException(number, "SMS is empty.", -12);
                case -13:
                    throw new SendSmsFailedException(number, "Invalid fake sender exceeded 12 chars or empty.", -13);
                case -25:
                    throw new SendSmsFailedException(number, "Sending rate greater than receiving rate (only for send/receive accounts).", -25);
                case -100:
                    throw new SendSmsFailedException(number, "other error.", -100);
            }
            return Task.FromResult(result);
        }
    }
}
