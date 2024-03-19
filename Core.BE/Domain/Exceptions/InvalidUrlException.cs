using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidUrlException : BusinessException
    {

        public InvalidUrlException(string url)
        {
            Code = (int)ErrorCodes.InvalidUrl;
            MessageEn = $"Invalid url: {url} ";
            MessageAr = $"الرابط غير سليم: {url}";
        }
    }
}