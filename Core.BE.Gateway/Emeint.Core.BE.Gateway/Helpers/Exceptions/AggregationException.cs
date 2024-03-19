using Emeint.Core.BE.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Helpers.Exceptions
{
    public class AggregationException : BusinessException
    {
        public AggregationException(int errorCode, string message, string errorDetails)
        {
            Code = errorCode;
            MessageEn = message;
            MessageAr = message;
            MoreDetails = errorDetails;
        }
    }
}
