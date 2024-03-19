using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Exceptions
{
    public class MessageTypeNotFoundException : BusinessException
    {
        public MessageTypeNotFoundException(int id)
        {
            Code = (int)MessagingErrorCodes.MessageTypeNotFound;
            MessageEn = "Can not update message status from " + id;
            MessageAr = "لا يمكنك تعديل حالة الرسالة من " + id;
        }
    }
}
