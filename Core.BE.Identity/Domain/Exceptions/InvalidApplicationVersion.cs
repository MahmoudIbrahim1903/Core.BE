using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gosour.Domain.Exceptions
{
    public class InvalidApplicationVersion : BusinessException
    {
        public InvalidApplicationVersion()
        {
            Code = (int)IdentityErrorCodes.InvalidApplicationVersion;
            MessageEn = "Invalid Application version!";
            MessageAr = "هذا الاصدار غير صحيح";
        }

    }
}
