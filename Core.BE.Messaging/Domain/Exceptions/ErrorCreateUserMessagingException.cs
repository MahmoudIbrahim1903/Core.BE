using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Exceptions
{
    public class ErrorCreateUserMessagingException : BusinessException
    {
        public ErrorCreateUserMessagingException()
        {
            Code = (int)MessagingErrorCodes.ErrorCreateUser;
            MessageEn = "Invalid Entry data";
            MessageAr = "لم يتم ادخال البيانات كاملة!";
        }
    }
}
