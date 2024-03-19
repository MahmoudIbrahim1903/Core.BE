using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Exceptions
{
    public class ErrorUpdateMessageStatusException : BusinessException
    {
        public ErrorUpdateMessageStatusException(Status oldStatus, Status newStatus)
        {
            Code = (int)MessagingErrorCodes.ErrorUpdateMessaageStatus;
            MessageEn = "Can not update message status from " + oldStatus + " To " + newStatus;
            MessageAr = "لا يمكنك تعديل حالة الرسالة من " + oldStatus + "الي " + newStatus;
        }
    }
}
