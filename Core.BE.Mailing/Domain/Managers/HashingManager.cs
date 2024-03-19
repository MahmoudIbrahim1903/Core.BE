using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Emeint.Core.BE.Mailing.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Mailing.Domain.Configurations;

namespace Emeint.Core.BE.Mailing.Domain.Managers
{
    public class HashingManager : IHashingManager
    {
        private readonly string _secureKey;
        public HashingManager(IConfigurationManager configurationManager)
        {
            _secureKey = configurationManager.GetIntermicroservicesHashKey();
        }

        private string HashMessage(string messageToHash)
        {
            return Emeint.Core.BE.Domain.Managers.HashingManager.HashMessage
                (messageToHash, new SHA256Managed(), _secureKey);
        }

        public string HashSendEmailRequest(SendMailViewModel mail)
        {
            if (mail == null)
                return String.Empty;

            var messageToHash = mail.To[0].ToLower().Trim() + mail.From.ToLower().Trim();
            return HashMessage(messageToHash);
        }

    }
}
