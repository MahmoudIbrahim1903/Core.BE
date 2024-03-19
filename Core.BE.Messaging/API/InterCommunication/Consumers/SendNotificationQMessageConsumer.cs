using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;

namespace Emeint.Core.BE.Notifications.API.InterCommunication.Consumers
{
    public class SendNotificationQMessageConsumer : IInterCommMessageConsumer<SendNotificationQMesssage>
    {
        private readonly IServerMessageService _serverMessageService;
        public SendNotificationQMessageConsumer(IServerMessageService serverMessageService)
        {
            _serverMessageService = serverMessageService;
        }

        public async Task Consume(ConsumeContext<SendNotificationQMesssage> context)
        {
            SendNotificationRequestDto messageFull = new SendNotificationRequestDto();

            messageFull.Alert = context.Message.Alert;
            messageFull.MessageTypeCode = context.Message.MessageTypeCode;
            messageFull.MessageSource = (MessageSource)context.Message.MessageSource;
            messageFull.DisplayParams = context.Message.DisplayParams;
            messageFull.DisplayParamsAr = context.Message.DisplayParamsAr;
            messageFull.DisplayParamsSw = context.Message.DisplayParamsSw;
            messageFull.TitleParams = context.Message.TitleParams;
            messageFull.TitleParamsAr = context.Message.TitleParamsAr;
            messageFull.TitleParamsSw = context.Message.TitleParamsSw;
            messageFull.Destinations = new List<Application.ValueObjects.Common.MessageDestinationsViewModel>();
            messageFull.SendAsPush = context.Message.SendAsPush;

            foreach (var item in context.Message.Destinations)
            {
                messageFull.Destinations.Add(new Application.ValueObjects.Common.MessageDestinationsViewModel
                { DestinationType = (DestinationType)item.DestinationType, Recipients = item.Recipients });
            }

            messageFull.ExtraParams = context.Message.ExtraParams;

            var result = await _serverMessageService.Send(messageFull);
        }
    }
}
