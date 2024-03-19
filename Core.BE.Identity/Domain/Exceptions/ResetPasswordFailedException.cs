using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class ResetPasswordFailedException : BusinessException
    {
        public ResetPasswordFailedException(string userEmail, string moreDetails)
        {
            Code = (int)IdentityErrorCodes.ResetPasswordFailed;
            MessageEn = $"Reseting password for user: {userEmail} failed";
            MessageAr = $": فشلت عملية استرجاع هذا المستخدم{userEmail}";

            MoreDetails = moreDetails;
        }
    }
}