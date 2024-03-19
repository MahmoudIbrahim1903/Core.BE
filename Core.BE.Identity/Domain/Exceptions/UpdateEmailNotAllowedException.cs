using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UpdateEmailNotAllowedException : BusinessException
    {
        public UpdateEmailNotAllowedException()
        {
            Code = (int)IdentityErrorCodes.UpdatedPhoneNumberNotAllowed;
            MessageEn = "Not allowed to update verified email";
            MessageAr = "غير مسموح لك بتعديل البريد الالكترونى بعد ان تم التحقق منه";
        }
    }
}