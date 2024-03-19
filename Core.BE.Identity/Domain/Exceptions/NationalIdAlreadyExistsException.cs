
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class NationalIdAlreadyExistsException : BusinessException
    {
        public NationalIdAlreadyExistsException()
        {
            Code = (int)IdentityErrorCodes.NationalIdAlreadyExists;
            MessageEn = "An account with this national ID already exists.";
            MessageAr = "يوجد حساب بنفس الرقم القومي! ";
        }
    }
}