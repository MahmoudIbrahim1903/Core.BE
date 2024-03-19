using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Contracts.Notifier
{
    public interface INotifierService<T> where T : class
    {
        Task NotifyAsync(T message);
    }
}
