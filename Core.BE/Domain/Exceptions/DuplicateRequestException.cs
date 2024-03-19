using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class DuplicateRequestException : BaseException
    {
        public DuplicateRequestException()
        {
            Code = (int)ErrorCodes.DuplicateRequest;
            MessageEn = "Duplicate request!";
            MessageAr = "طلب مكرر";
        }
    }
}