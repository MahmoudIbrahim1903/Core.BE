using Emeint.Core.BE.SMS.Application.ViewModels;
using Emeint.Core.BE.SMS.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Application.Mappers
{
    public static class SmsMessageMapper
    {
        public static SmsMessage MapToSmsMessage(SmsViewModel sms, int templateId, string messageText)
        {
            var smsMessage = new SmsMessage(sms.CreatedBy)
            {
                MessageText = messageText,
                MessageTemplateId = templateId,
                SmsProviderType = sms.SmsProvider
            };

            return smsMessage;
        }
    }
}
