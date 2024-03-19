using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.Mailing.Domain.Managers;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.API.InterCommunication.Consumers
{
    public class SendingEmailQMessageConsumer : IInterCommMessageConsumer<EmailQMessage>
    {
        private readonly IEmailManager _emailManager;
        private readonly ILogger<SendingEmailQMessageConsumer> _logger;

        public SendingEmailQMessageConsumer(IEmailManager emailManager, ILogger<SendingEmailQMessageConsumer> logger)
        {
            _emailManager = emailManager;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EmailQMessage> context)
        {

            _logger.LogInformation($"Start Consuming Message from:{context.Message.From}, to:{context.Message.To.FirstOrDefault()},subject:{context.Message.Subject}, date:{DateTime.UtcNow} ");

            await _emailManager.SendEmail(context.Message.From, context.Message.FromDisplayName, context.Message.To, context.Message.Cc,
               context.Message.Subject, context.Message.Body, context.Message.Attachments, context.Message.IsImportant,
               context.Message.Type, context.Message.ExtraParams);

            _logger.LogInformation($"End Consuming Message from:{context.Message.From}, to:{context.Message.To.FirstOrDefault()},subject:{context.Message.Subject}, date:{DateTime.UtcNow} ");
        }
    }
}
