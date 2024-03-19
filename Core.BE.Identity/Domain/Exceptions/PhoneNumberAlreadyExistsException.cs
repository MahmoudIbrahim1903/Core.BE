
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class PhoneNumberAlreadyExistsException : BusinessException
    {
        public PhoneNumberAlreadyExistsException()
        {
            Code = (int)IdentityErrorCodes.PhoneNumberAlreadyExists;
            MessageEn = "An account with this phone number already exists.";
            MessageAr = "يوجد حساب برقم الهاتف هذا ";
        }
    }
}