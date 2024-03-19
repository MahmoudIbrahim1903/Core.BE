using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class InvalidResetPasswordOtpException : BusinessException
    {
        public InvalidResetPasswordOtpException()
        {
            Code = (int)IdentityErrorCodes.InvalidResetPasswordOtp;
            Resourcekey = IdentityErrorCodes.InvalidResetPasswordOtp.ToString();
            MessageEn = "Invalid reset password code!";
            MessageAr = "الكود المستخدم لإعادة تعيين كلمة المرور غير صحيح!";
        }
    }
}
