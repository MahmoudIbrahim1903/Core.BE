
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class RemovePasswordFailedException : BusinessException
    {
        public RemovePasswordFailedException(string userEmail,string moreDetails)
        {
            Code = (int)IdentityErrorCodes.RemovePasswordFailed;
            MessageEn = $"Removing password for user: {userEmail} failed";
            MessageAr = $"فشلت عملية حذف هذا المستخدم: {userEmail}";
            MoreDetails = moreDetails;
        }
    }
}