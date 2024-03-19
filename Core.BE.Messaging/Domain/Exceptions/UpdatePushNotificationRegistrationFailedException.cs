using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Exceptions
{
    public class UpdatePushNotificationRegistrationFailedException : BusinessException
    {
        public UpdatePushNotificationRegistrationFailedException()
        {
            Code = (int)MessagingErrorCodes.UpdatePushNotificationRegistrationFailed;
            MessageEn = "Failed to create or update push notification registration!";
            MessageAr = "لم نتمكن من إنشاء أو تعديل التسجيل في الإشعارات!";
        }
    }
}
