using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidPhoneNumberException : BusinessException
    {
        public InvalidPhoneNumberException(string phoneNumber)
        {
            Code = (int)ErrorCodes.InvalidPhoneNumber;
            Resourcekey = ErrorCodes.InvalidPhoneNumber.ToString();
            MessageParameters.Add(phoneNumber);
            MessageEn = "Invalid phone number: " + phoneNumber;
            MessageAr = "رقم الهاتف غير سليم: " + phoneNumber;
        }
    }
}