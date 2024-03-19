using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.SMS.Application.Mappers;
using Emeint.Core.BE.SMS.Application.ViewModels;
using Emeint.Core.BE.SMS.Infractructure.Data;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Enums;
using Emeint.Core.BE.SMS.Domain.Exceptions;
using Emeint.Core.BE.SMS.Domain.SmsProviders;
using Emeint.Core.BE.SMS.Infractructure.Model;
using Emeint.Core.BE.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.SMS.Domain.Managers
{
    public class SmsMessageManager : ISmsMessageManager
    {

        private readonly SmsFactory _smsFactory;
        private readonly IMessageTemplateRepository _messageTemplateRepository;
        private readonly ISmsMessageRepository _smsMessageRepository;
        private readonly IUserManager _userManager;
        private readonly IConfigurationManager _configurationManager;
        private readonly ILogger _logger;


        public SmsMessageManager(IConfigurationManager configurationManager,
            IIdentityService identityService, IMessageTemplateRepository messageTemplateRepository,
            ISmsMessageRepository smsMessageRepository, IUserManager userManager, ILogger<SmsMessageManager> logger, IWebRequestUtility webRequestUtility)
        {
            _smsFactory = new SmsFactory(configurationManager, identityService, webRequestUtility);
            _configurationManager = configurationManager;
            _messageTemplateRepository = messageTemplateRepository;
            _smsMessageRepository = smsMessageRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(SmsViewModel sms)
        {
            if (sms == null)
                throw new MissingParameterException("sms");

            if (sms.PhoneNumbers == null || sms.PhoneNumbers.Count() == 0)
                throw new MissingParameterException("phone_numbers");

            var smsMessage = ConcatenateMessageParts(sms);

            _smsMessageRepository.SaveSmsMessage(smsMessage);

            if (string.IsNullOrEmpty(smsMessage.MessageText))
                throw new EmptyMessageException();

            var smsProvider = _smsFactory.GetSMSProvider(sms.SmsProvider);

            foreach (var phoneNumber in sms.PhoneNumbers)
            {
                _logger.LogInformation("message_id:{0}, user_phone_number:{1}, message_text:{2}, smsProvider:{3}",
                    smsMessage.Id, phoneNumber, smsMessage.MessageText, smsMessage.SmsProviderType.ToString());

                int statusCode = 0;
                string statusMessage = String.Empty;

                try
                {
                    statusCode = await smsProvider.SendSmsAsync(phoneNumber, smsMessage.MessageText);
                    statusMessage = "Success";
                }
                catch (SendSmsFailedException ex)
                {
                    statusCode = ex.StatusCode;
                    statusMessage = ex.MoreDetails;
                }

                _logger.LogInformation("message_id:{0}, user_phone_number:{1}, message_text:{2}, smsProvider:{3}, status_code:{4}, status_message:{5}",
                    smsMessage.Id, phoneNumber, smsMessage.MessageText, smsMessage.SmsProviderType.ToString(), statusCode, statusMessage);


                var user = _userManager.AddUser(phoneNumber, sms.CreatedBy);
                var messageUser = _smsMessageRepository.AddMessageUser(smsMessage.Id, user.Id, sms.CreatedBy);
                _smsMessageRepository.AddMessageStatus(messageUser.Id, statusCode, statusMessage, sms.CreatedBy);
            }

            return true;
        }

        public SmsMessage ConcatenateMessageParts(SmsViewModel sms)
        {
            _logger.LogInformation(" sms_template_code:{1}, message_parameters:{2}",
                sms.TemplateCode,
               (sms.MessageParameters != null && sms.MessageParameters.Count > 0) ?
               string.Join(',', sms.MessageParameters.Select(p => p.Value)?.ToList()) : string.Empty);

            string message = string.Empty;

            if (string.IsNullOrEmpty(sms.TemplateCode))
                throw new MissingParameterException("template_code");

            var messageTemplate = _messageTemplateRepository.GetTemplateByCode(sms.TemplateCode);

            if (messageTemplate == null)
                throw new MessageTemplateNotFoundException(sms.TemplateCode);

            var templateId = messageTemplate.Id;

            Language messageLanguage = string.IsNullOrEmpty(sms.Language) ?
                Language.en : Enum.Parse<Language>(sms.Language);

            message = LocalizationUtility.GetLocalizedText
                (messageTemplate.ContentEn, messageTemplate.ContentAr, messageLanguage, messageTemplate.ContentSw);

            if (sms.MessageParameters != null && sms.MessageParameters.Any())
                message = string.Format(message, sms.MessageParameters.Select(p => p.Value).ToArray());

            return SmsMessageMapper.MapToSmsMessage(sms, templateId, message);
        }


    }
}
