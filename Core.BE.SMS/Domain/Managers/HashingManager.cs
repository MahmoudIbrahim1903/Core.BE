using Emeint.Core.BE.SMS.Application.ViewModels;
using Emeint.Core.BE.SMS.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Managers
{
    public class HashingManager : IHashingManager
    {
        private string SECURE_KEY;
        public HashingManager(IConfigurationManager configurationManager)
        {
            SECURE_KEY = configurationManager.GetIntermicroservicesHashKey();
        }

        public string HashSendSmsRequest(SmsViewModel smsViewModel)
        {
            if (smsViewModel == null)
                return String.Empty;

            string messageToHash =
                 smsViewModel.PhoneNumbers.FirstOrDefault().ToString().Trim() +
                 smsViewModel.SmsProvider.ToString().Trim();

            return HashMessage(messageToHash);
        }

        private string HashMessage(string messageToHash)
        {
            return Emeint.Core.BE.Domain.Managers.HashingManager.HashMessage
                (messageToHash, new SHA256Managed(), SECURE_KEY);
        }


    }
}
