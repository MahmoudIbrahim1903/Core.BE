using Emeint.Core.BE.SMS.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Managers
{
    public interface ISmsMessageManager
    {
        Task<bool> SendSmsAsync(SmsViewModel smsViewModel);
    }
}
