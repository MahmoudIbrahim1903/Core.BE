using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidOperationException : BusinessException
    {
        public InvalidOperationException()
        {
            Code = (int)ErrorCodes.InvalidOperation;
            Resourcekey = ErrorCodes.InvalidOperation.ToString();
            MessageEn = "Invalid Operation";
            MessageAr = "عملية غير مقبولة";
        }

        public InvalidOperationException(string resourceKey)
        {
            Code = (int)ErrorCodes.InvalidOperation;
            Resourcekey = resourceKey;
        }


        public InvalidOperationException(string detailsEn, string detailsAr)
        {
            Code = (int)ErrorCodes.InvalidOperation;
            MessageEn = "Invalid Operation: " + detailsEn;
            MessageAr = "عملية غير مقبولة: " + detailsAr;
        }
    }
}