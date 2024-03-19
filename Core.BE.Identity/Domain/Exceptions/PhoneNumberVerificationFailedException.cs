
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class PhoneNumberVerificationFailedException : BusinessException
    {
        public PhoneNumberVerificationFailedException() : this(null)
        {
        }

        public PhoneNumberVerificationFailedException(string moreDetails)
        {
            Code = (int)IdentityErrorCodes.PhoneNumberVerificationFailed;
            MessageEn = "Phone number verification failed!";
            MessageAr = "تأكيد رقم الهاتف فشل";
            MoreDetails = moreDetails;
        }
    }
}