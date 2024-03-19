using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class EmailAlreadyVerifiedException : BusinessException
    {
        public EmailAlreadyVerifiedException()
        {
            Code = (int)IdentityErrorCodes.EmailAlreadyVerified;
            MessageEn = "Email already verified!";
            MessageAr = "تم تأكيد البريد الالكترونى مسبقاً";
        }
    }
}