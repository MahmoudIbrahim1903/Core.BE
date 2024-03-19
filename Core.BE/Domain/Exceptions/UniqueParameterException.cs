using Emeint.Core.BE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class UniqueParameterException : BusinessException
    {
        public UniqueParameterException(string parameterName) : this(parameterName, null)
        {
        }

        public UniqueParameterException(string parameterName, string moreDetails)
        {
            Code = (int)ErrorCodes.InvalidUniqueParameter;
            MessageEn = string.Format($"{parameterName} already exists");
            MessageAr = string.Format($"{parameterName} موجود بالفعل");
            MoreDetails = moreDetails;
        }
    }
}
