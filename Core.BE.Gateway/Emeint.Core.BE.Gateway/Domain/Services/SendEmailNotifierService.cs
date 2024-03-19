using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.Gateway.Domain.Contracts;
using Emeint.Core.BE.InterCommunication.Concretes.Notifier;

namespace Gateway.Domain.Services
{
    public class SendEmailNotifierService : NotifierServiceBase<EmailQMessage>, ISendEmailNotifierService
    {
        public SendEmailNotifierService(IBusMessageSender messageSender)
          : base(messageSender, "ServiceRequired.SendEmail")
        {
        }

    }
}
