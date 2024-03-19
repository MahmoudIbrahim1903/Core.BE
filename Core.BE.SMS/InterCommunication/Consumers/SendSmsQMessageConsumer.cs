using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.SMS.Domain.Managers;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.InterCommunication.Consumers
{
    public class SendSmsQMessageConsumer : IInterCommMessageConsumer<SmsQMessage>
    {
        private readonly ISmsMessageManager _smsMessageManager;
        public SendSmsQMessageConsumer(ISmsMessageManager smsMessageManager)
        {
            _smsMessageManager = smsMessageManager;
        }
        public async Task Consume(ConsumeContext<SmsQMessage> context)
        {
            var message = context.Message;
            await _smsMessageManager.SendSmsAsync(new Application.ViewModels.SmsViewModel()
            {
                CreatedBy = message.CreatedBy,
                Language = message.Language,
                MessageParameters = message.MessageParameters,
                PhoneNumbers = message.PhoneNumbers,
                SmsProvider = (Domain.Enums.SmsProvider)message.SmsProvider,
                TemplateCode = message.TemplateCode
            });
        }
    }
}
