using Emeint.Core.BE.InterCommunication.Contracts.Notifier;
using Emeint.Core.BE.InterCommunication.Messages;

namespace Emeint.Core.BE.Gateway.Domain.Contracts
{
    public interface ISendEmailNotifierService : INotifierService<EmailQMessage>
    {
    }
}
