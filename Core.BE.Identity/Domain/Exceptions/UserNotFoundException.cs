
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UserNotFoundException : BusinessException
    {
        public UserNotFoundException()
        {
            Code = (int)IdentityErrorCodes.UserNotFound;
            Resourcekey = IdentityErrorCodes.UserNotFound.ToString();
            MessageEn = "User does not exist!";
            MessageAr = "المستخدم غير موجود!";
        }
    }
}