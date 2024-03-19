using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.SMS.Application.ViewModels;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Exceptions;
using Emeint.Core.BE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.SmsProviders
{
    public class ArpuSmsProvider : ISmsSender
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IWebRequestUtility _webRequestUtility;
        public ArpuSmsProvider(IConfigurationManager configurationManager, IWebRequestUtility webRequestUtility)
        {
            _configurationManager = configurationManager;
            _webRequestUtility = webRequestUtility;
        }

        //TODo: handle Get message status API

        public async Task<int> SendSmsAsync(string number, string message)
        {
            var userName = _configurationManager.GetArpuUserName(); 
            var userPassword = _configurationManager.GetArpuPassword(); 
            var accountId = _configurationManager.GetArpuAccountId(); 
            var url = _configurationManager.GetArpuSingleSmsUrl(); //https://api.connekio.com/sms/single
            
            string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{userPassword}:{accountId}"), Base64FormattingOptions.InsertLineBreaks);

            var response = await _webRequestUtility.Post<ArpuSendSmsResponseVm>(
               new HttpPostRequest(
                   url,
                   new
                   {
                       account_id = accountId,
                       text = message,
                       msisdn = number.TrimStart('+').TrimStart('0'),
                       sender = _configurationManager.GetArpuSMSSender()
                   },
                   "application/json",
                   new List<KeyValuePair<string, string>>()
                   {
                        new KeyValuePair<string, string>("Authorization", $"Basic {encodedToken}")
                   },
                   JsonNamingStrategy.SnakeCase),
               JsonNamingStrategy.SnakeCase);

            if (response.Status == true)
                return 0; //succes
            else
                throw new SendSmsFailedException(number, response.StatusDescription, 1);
        }
    }
}
