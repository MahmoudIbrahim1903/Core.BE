using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidGuidException : BusinessException
    {
        public InvalidGuidException(string guidValue)
        {
            Code = (int)ErrorCodes.InvalidGuid;
            MessageEn = string.Format("Invalid GUID value: " + guidValue);
            MessageAr = string.Format(" كود الطلب غير سليم: " + guidValue);
        }
    }
}