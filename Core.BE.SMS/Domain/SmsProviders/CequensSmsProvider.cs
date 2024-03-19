using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.SMS.Application.ViewModels;
using Emeint.Core.BE.SMS.Application.ViewModels.Cequens;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Exceptions;
using Emeint.Core.BE.Utilities;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.SmsProviders
{
    public class CequensSmsProvider : ISmsSender
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IWebRequestUtility _webRequestUtility;

        public CequensSmsProvider(IConfigurationManager configurationManager, IWebRequestUtility webRequestUtility)
        {
            _configurationManager = configurationManager;
            _webRequestUtility = webRequestUtility;
        }

        public async Task<int> SendSmsAsync(string number, string message)
        {
            var token = _configurationManager.GetCequensToken();
            var url = _configurationManager.GetCequensSendSmsUrl();
            var senderName = _configurationManager.GetCequensSenderName();

            var request = new HttpPostRequest(url, new
            {
                messageText = message,
                senderName = senderName,
                messageType = "text", //requiredByAPI
                recipients = number
            },
            "application/json",
            new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Authorization", $"Bearer {token}"),
                new KeyValuePair<string, string>("accept", "application/json"),
            },
            JsonNamingStrategy.CamelCase
            );

            try
            {
                var response = await _webRequestUtility.Post<CequensSendSmsResponseVm>(request, JsonNamingStrategy.CamelCase);
                if (response.ReplyCode == 0)
                    return 0; //succes
                else
                    throw new SendSmsFailedException(number, response.ReplyMessage, response.ReplyCode);
            }
            catch(RequestToProviderFailedException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.BadRequest) //known response 
                {
                    var response = Deserialization.JsonStringToObject<CequensSendSmsResponseVm>(ex.OriginalContent, JsonNamingStrategy.CamelCase, MissingMemberHandling.Ignore);
                    throw new SendSmsFailedException(number, response.ReplyMessage, response.ReplyCode);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
