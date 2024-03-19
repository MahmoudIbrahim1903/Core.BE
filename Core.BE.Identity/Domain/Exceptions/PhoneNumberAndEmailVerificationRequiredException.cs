
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class PhoneNumberAndEmailVerificationRequiredException : BusinessException
    {
        public PhoneNumberAndEmailVerificationRequiredException()
        {
            Code = (int)IdentityErrorCodes.PhoneNumberAndEmailVerificationRequired;
            MessageEn = "Phone number and email verification required";
            MessageAr = "يلزم تأكيد رقم المحمول والبريد الالكترونى";
        }
    }
}