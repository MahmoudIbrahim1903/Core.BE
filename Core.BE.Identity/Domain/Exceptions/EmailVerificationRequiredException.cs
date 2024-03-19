
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class EmailVerificationRequiredException : BusinessException
    {
        public EmailVerificationRequiredException()
        {
            Code = (int)IdentityErrorCodes.EmailVerificationRequired;
            Resourcekey = IdentityErrorCodes.EmailVerificationRequired.ToString();
            MessageEn = "Email verification required";
            MessageAr = "يلزم تأكيد البريد الالكترونى";
        }
    }
}