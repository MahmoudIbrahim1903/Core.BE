using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class InvalidPasswordException : BusinessException
    {
        
        public InvalidPasswordException()
        {
            Code = (int)IdentityErrorCodes.InvalidPassword;
            MessageEn = "Invalid password";
            MessageAr = "كلمة المرور غير صحيحة";
        }
    }
}