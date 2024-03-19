
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class InvalidUsernameException : BusinessException
    {
        public InvalidUsernameException()
        {
            Code = (int)IdentityErrorCodes.InvalidUsername;
            MessageEn = "Invalid username";
            MessageAr = "اسم المستخدم غير صحيح";

        }
    }
}