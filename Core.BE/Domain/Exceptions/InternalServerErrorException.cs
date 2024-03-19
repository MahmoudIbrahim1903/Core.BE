using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InternalServerErrorException : RuntimeException
    {
        public InternalServerErrorException() : this(null)
        {
        }

        public InternalServerErrorException(string moreDetails)
        {
            Code = (int)ErrorCodes.InternalServerError;
            Resourcekey = ErrorCodes.InternalServerError.ToString();
            MessageEn = "Internal server error";
            MessageAr = "خطأ فى الخادم";
            MoreDetails = moreDetails;
        }
    }
}