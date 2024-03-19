using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class EmailAlreadyExistsException : BusinessException
    {
        public EmailAlreadyExistsException()
        {
            Code = (int)IdentityErrorCodes.EmailAlreadyExists;
            MessageEn = "Email already exists";
            MessageAr = "البريد الالكترونى موجود مسبقاً";

        }

        public EmailAlreadyExistsException(string email)
        {
            Code = (int)IdentityErrorCodes.EmailAlreadyExists;
            MessageEn = $"Email already exists: {email}!";
            MessageAr = "البريد الالكترونى موجود مسبقاً" + $": {email}!";

        }
    }
}
