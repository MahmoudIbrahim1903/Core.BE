using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.SmsProviders
{
    public interface ISmsSender
    {
        Task<int> SendSmsAsync(string number, string message);
    }
}
