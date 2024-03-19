using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.SMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Exceptions
{
    public class EmptyMessageException   :BusinessException
    {
        public EmptyMessageException()
        {
            Code = (int)SmsErrorCodes.EmptyMessage;
            MessageEn = $"You can not send an empty SMS ";
            MessageAr = $"لا يمكنك ارسال رسالة فارغة";
        }
    }
}
