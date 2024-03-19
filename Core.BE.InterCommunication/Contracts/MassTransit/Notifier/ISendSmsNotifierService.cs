using Emeint.Core.BE.InterCommunication.Concretes.Notifier;
using Emeint.Core.BE.InterCommunication.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Contracts.Notifier
{
    public interface ISendSmsNotifierService : INotifierService<SmsQMessage>
    {
    }
}
