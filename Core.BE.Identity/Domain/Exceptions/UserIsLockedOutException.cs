using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UserIsLockedOutException : BusinessException
    {
        public UserIsLockedOutException()
        {
            Code = (int)IdentityErrorCodes.UserIsLockedOutException;
            Resourcekey = IdentityErrorCodes.UserIsLockedOutException.ToString();
            MessageEn = "User is locked out!";
            MessageAr = "هذا الحساب موقوف!";
        }
    }
}
