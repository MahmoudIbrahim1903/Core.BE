using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.SMS.Domain.Enums;

namespace Emeint.Core.BE.SMS.Domain.Exceptions
{
    public class SendSmsFailedException : BusinessException
    {
        public int StatusCode { get; set; }
        public SendSmsFailedException(string number, string moreDetails, int statusCode)
        {
            Code = (int)SmsErrorCodes.SendSmsFailed;
            MessageEn = $"Sending SMS to  {number } failed";
            MessageAr = $"فشلت عملية ارسال رسالة قصيرة لهذا الرقم {number}";
            MoreDetails = moreDetails;
            StatusCode = statusCode;
        }
    }
}
