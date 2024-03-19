using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.External;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using Emeint.Core.BE.Notifications.Domain.Configurations;

namespace Emeint.Core.BE.Notifications.Domain.Manager
{
    public class HashingManager : IHashingManager
    {
        private string _secureKey;// = "kjsdfiusef;7yewrhas-cds:sqd";

        public HashingManager(IConfigurationManager configurationManager)
        {
            _secureKey = configurationManager.GetIntermicroservicesHashKey();
        }

        public string HashSendMessageRequest(SystemEventNotificationDto serverVM)
        {
            if (serverVM == null)
                return String.Empty;

            var messageToHash = serverVM.Alert.ToString().ToLower().Trim() +
                serverVM.Destinations.Count().ToString().ToLower().Trim() +
                serverVM.Destinations[0].DestinationType.ToString().ToLower().Trim();//ToBe:discussed 
            return HashMessage(messageToHash);
        }

        public string HashSendMessageRequest(ExternalMessageViewModel serverVM)
        {
            if (serverVM == null)
                return String.Empty;

            var messageToHash = serverVM.Destinations.Recipients.Count().ToString().ToLower().Trim() +
                          serverVM.Destinations.DestinationType.ToString().ToLower().Trim();
            return HashMessage(messageToHash);
        }
        private string HashMessage(string messageToHash)
        {
            return Emeint.Core.BE.Domain.Managers.HashingManager.HashMessage
                (messageToHash, new SHA256Managed(), _secureKey);
        }
    }
}
