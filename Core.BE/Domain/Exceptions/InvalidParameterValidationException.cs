using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidParameterValidationException : BusinessException
    {
        public InvalidParameterValidationException(string message)
        {
            Code = 100;
            MessageEn = message;
            MessageAr = message;
        }
    }
}
