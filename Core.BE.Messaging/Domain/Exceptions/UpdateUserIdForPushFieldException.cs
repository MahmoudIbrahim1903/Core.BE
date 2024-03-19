using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Exceptions
{
    public class UpdateUserIdForPushFieldException : BusinessException
    {
        public UpdateUserIdForPushFieldException(string userId)
        {
            Code = (int)MessagingErrorCodes.InvalidUserId;
            MessageEn = "Can not send " + userId + " with null value";
            MessageAr = "بدون بيانات " + userId + "غير مسموح ارسال";
        }
    }
}
