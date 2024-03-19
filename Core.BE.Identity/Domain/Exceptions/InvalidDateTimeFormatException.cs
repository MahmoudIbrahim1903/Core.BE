using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class InvalidDateTimeFormatException : BusinessException
    {
        public InvalidDateTimeFormatException()
        {
            Code = (int)IdentityErrorCodes.DateTimeFormatException;
            MessageEn = "Invalid Format For Date Time";
            MessageAr = "";
        }
    }
}