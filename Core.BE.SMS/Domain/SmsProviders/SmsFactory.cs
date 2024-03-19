using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Enums;
using Emeint.Core.BE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.SmsProviders
{
    public class SmsFactory
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IIdentityService _identityService;
        private readonly IWebRequestUtility _webRequestUtility;

        public SmsFactory(IConfigurationManager configuartionManager, IIdentityService identityService, IWebRequestUtility webRequestUtility)
        {
            _configurationManager = configuartionManager;
            _identityService = identityService;
            _webRequestUtility = webRequestUtility;
        }
        public ISmsSender GetSMSProvider(SmsProvider providerType)
        {
            switch (providerType)
            {
                case SmsProvider.EZagel:
                    return new EZagelSmsProvider(_configurationManager);
                case SmsProvider.VictoryLinkKannel:
                    return new VictoryLinkKannelSmsProvider(_configurationManager, _identityService);
                case SmsProvider.Twilio:
                    return new TwilioSmsProvider(_configurationManager);
                case SmsProvider.Arpu:
                    return new ArpuSmsProvider(_configurationManager, _webRequestUtility);
                case SmsProvider.VictoryLinkReseller:
                    return new VictoryLinkResellerSmsProvider(_configurationManager, _identityService);
                case SmsProvider.Cequens:
                    return new CequensSmsProvider(_configurationManager,_webRequestUtility);
                default:
                    return new TwilioSmsProvider(_configurationManager);
            }
        }
    }
}
