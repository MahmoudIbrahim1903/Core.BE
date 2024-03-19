using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidEmailException : BusinessException
    {

        public InvalidEmailException(string email)
        {
            Code = (int)ErrorCodes.InvalidEmail;
            MessageEn = "Invalid email: " + email;
            MessageAr = "البريد الإلكترونى غير سليم:" + email;
        }
    }
}