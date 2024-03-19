using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts.InterCommunication;
using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Concretes.InterCommunication
{
    public class SendEmailNotifierService : NotifierServiceBase<EmailQMessage>, ISendEmailNotifierService
    {
        public SendEmailNotifierService(IBusMessageSender messageSender)
          : base(messageSender, "ServiceRequired.SendEmail")
        {
        }
    }
}
