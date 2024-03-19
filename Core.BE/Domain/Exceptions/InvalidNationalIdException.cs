using Emeint.Core.BE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidNationalIdException : BusinessException
    {
        public InvalidNationalIdException(string nationalId)
        {
            Code = (int)ErrorCodes.InvalidNationalId;
            MessageEn = $"Invalid national ID:{nationalId}";
            MessageAr = "الرقم القومي غير صحيح:" + nationalId;
        }
    }
}