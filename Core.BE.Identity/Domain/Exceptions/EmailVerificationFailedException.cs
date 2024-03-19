
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class EmailVerificationFailedException : BusinessException
    {
        public EmailVerificationFailedException() : this(null)
        {
        }

        public EmailVerificationFailedException(string moreDetails)
        {
            Code = (int)IdentityErrorCodes.EmailVerificationFailed;
            MessageEn = "Email verification failed!";
            MessageAr = "تأكيد البريد الالكترونى فشل!";
            MoreDetails = moreDetails;
        }
    }
}