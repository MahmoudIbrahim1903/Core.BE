using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UpdatePhoneNumberNotAllowedException : BusinessException
    {
        public UpdatePhoneNumberNotAllowedException()
        {
            Code = (int)IdentityErrorCodes.UpdatedPhoneNumberNotAllowed;
            MessageEn = "Not allowed to update verified phone number";
            MessageAr = "غير مسموح لك بتعديل رقم الهاتف بعد ان تم التحقق منه";
        }
    }
}