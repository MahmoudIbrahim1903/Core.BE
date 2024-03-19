
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class PhoneNumberVerificationRequiredException : BusinessException
    {
        public PhoneNumberVerificationRequiredException()
        {
            Code = (int)IdentityErrorCodes.PhoneNumberVerificationRequired;
            Resourcekey = IdentityErrorCodes.PhoneNumberVerificationRequired.ToString();
            MessageEn = "Phone number verification required";
            MessageAr = "مطلوب تأكيد رقم المحمول";
        }
    }
}