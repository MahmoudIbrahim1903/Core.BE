using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Contracts.Notifier
{
    public interface IRespondingNotifierService<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        Task<TResponse> NotifyAndGetResponseAsync(TRequest message);

    }
}
