using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class PhoneNumberAlreadyVerifiedException : BusinessException
    {
        public PhoneNumberAlreadyVerifiedException()
        {
            Code = (int)IdentityErrorCodes.PhoneNumberAlreadyVerified;
            MessageEn= "Phone number already verified!";
            MessageAr = "تم تأكيد رقم الهاتف مسبقاً!";
        }
    }
}