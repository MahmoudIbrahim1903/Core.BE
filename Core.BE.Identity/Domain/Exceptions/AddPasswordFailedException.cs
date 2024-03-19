
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class AddPasswordFailedException : BusinessException
    {
        public AddPasswordFailedException(string userEmail, string moreDetails)
        {
            Code = (int)IdentityErrorCodes.AddPasswordFailed;
            MessageEn = $"Adding password for user {userEmail} failed";
            MessageAr = $"فشلت اضافة كلمة مرور لهذا المستخدم {userEmail} ";
            MoreDetails = moreDetails;
        }
    }
}