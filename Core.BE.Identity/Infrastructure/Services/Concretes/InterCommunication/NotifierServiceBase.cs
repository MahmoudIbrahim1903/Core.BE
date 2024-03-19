using Emeint.Core.BE.InterCommunication.Concretes.Base;
using Emeint.Core.BE.InterCommunication.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Concretes.InterCommunication
{
    public abstract class NotifierServiceBase<T> : BusInterCommunicatorBase where T : class
    {
        public NotifierServiceBase(IBusMessageSender messageSender, string destination)
            : base(messageSender, destination)
        {
        }

        public async Task NotifyAsync(T message)
        {
            await SendAndForget(message);
        }
    }
}
