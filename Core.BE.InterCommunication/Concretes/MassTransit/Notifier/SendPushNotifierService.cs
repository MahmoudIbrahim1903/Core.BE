using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Contracts.Notifier;
using Emeint.Core.BE.InterCommunication.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Concretes.Notifier
{
    public class SendPushNotifierService : NotifierServiceBase<SendNotificationQMesssage>, ISendPushNotifierService
    {
        public SendPushNotifierService(IBusMessageSender messageSender)
          : base(messageSender, "ServiceRequired.SendPushNotification")
        {
        }
    }
}
