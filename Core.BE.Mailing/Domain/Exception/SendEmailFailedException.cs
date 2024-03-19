using System;
using System.Text;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Mailing.Domain.Enums;

namespace Emeint.Core.BE.Mailing.Domain.Exceptions
{
    public class SendEmailFailedException : BusinessException
    {
        public SendEmailFailedException() : this( string.Empty)
        {
        }
        public SendEmailFailedException( string details)

        {
            Code = (int)ErrorCodes.SendEmailFailed;
            MessageEn = "Sending email failed, Please try again later!";
            MessageAr = "لقد فشل ارسال البريد الالكتروني ، من فضلك حاول في وقت لاحق!";
            MoreDetails = details;
        }
    }
}
