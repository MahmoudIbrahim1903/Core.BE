using Emeint.Core.BE.InterCommunication.Concretes.Base;
using Emeint.Core.BE.InterCommunication.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.Notifier
{
    public class RespondingNotifierServiceBase<TRequest, TResponse> : BusInterCommunicatorBase
        where TRequest : class
        where TResponse : class
    {
        public RespondingNotifierServiceBase(IBusMessageSender messageSender, string destination)
            : base(messageSender, destination)
        {

        }

        public async Task<TResponse> NotifyAndGetResponseAsync(TRequest message)
        {
            return await base.SendAndGetResponse<TRequest, TResponse>(message);
        }

    }
}
