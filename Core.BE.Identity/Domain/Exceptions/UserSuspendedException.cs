using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UserSuspendedException : BusinessException
    {
        public UserSuspendedException()
        {
            Code = (int)IdentityErrorCodes.UserSuspended;
            Resourcekey = IdentityErrorCodes.UserSuspended.ToString();
            MessageEn = "Account suspended!";
            MessageAr = "الحساب معلق!";
        }
    }
}