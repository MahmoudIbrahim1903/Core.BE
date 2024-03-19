using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class InvalidUsernameOrPasswordException : BusinessException
    {
        public InvalidUsernameOrPasswordException()
        {
            Code = (int)IdentityErrorCodes.InvalidUsernameOrPassword;
            Resourcekey = IdentityErrorCodes.InvalidUsernameOrPassword.ToString();
            MessageEn = "Invalid username or password!";
            MessageAr = "اسم المستخدم أو كلمة المرور غير صحيحة!";
        }
    }
}