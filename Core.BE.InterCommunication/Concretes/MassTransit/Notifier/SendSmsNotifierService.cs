using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Contracts.Notifier;
using Emeint.Core.BE.InterCommunication.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.Notifier
{
    public class SendSmsNotifierService : NotifierServiceBase<SmsQMessage>, ISendSmsNotifierService
    {
        public SendSmsNotifierService(IBusMessageSender messageSender)
          : base(messageSender, "ServiceRequired.SendSms")
        {
        }
    }
}
