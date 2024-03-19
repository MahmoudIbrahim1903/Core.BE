using Emeint.Core.BE.SMS.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Emeint.Core.BE.SMS.Domain.SmsProviders
{
    public class TwilioSmsProvider : ISmsSender

    {
        private readonly IConfigurationManager _configurationManager;
        public TwilioSmsProvider(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }
        public Task<int> SendSmsAsync(string number, string message)
        {
            string accountSid = _configurationManager.GetTwilioAccountSId();//"AC768acea103e19322348c513f172d57f2";
            string authToken = _configurationManager.GetTwilioAuthToken(); //"e9bc0abb11e487bddad45c42df6cf6b2";

            string fromPhoneNumber = _configurationManager.GetTwilioFromPhoneNumber();
            TwilioClient.Init(accountSid, authToken);

            var smsMessage = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(fromPhoneNumber),//+201156530465
                to: new Twilio.Types.PhoneNumber(number) //+201205940999
            );
            return Task.FromResult(smsMessage.ErrorCode.HasValue ? smsMessage.ErrorCode.Value : 0);

        }
    }
}
