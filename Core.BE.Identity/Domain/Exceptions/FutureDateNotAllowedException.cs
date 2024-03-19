using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class FutureDateNotAllowedException : BusinessException
    {
        public FutureDateNotAllowedException(DateTime date)
        {
            Code = (int)IdentityErrorCodes.FutureDateNotAllowed;
            MessageEn = "Date can not be in future: " + date;
            MessageAr = "التاريخ لا يمكن أن يكون في المستقبل: " + date;
        }
    }
}
