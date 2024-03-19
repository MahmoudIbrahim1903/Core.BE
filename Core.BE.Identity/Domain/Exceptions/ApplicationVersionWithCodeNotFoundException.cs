using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class ApplicationVersionWithCodeNotFoundException: BusinessException
    {
        public ApplicationVersionWithCodeNotFoundException()
        {
            Code = (int)IdentityErrorCodes.ApplicationVersionWithCodeNotFound;
            MessageEn = "Application version does not exist!";
            MessageAr = "هذا الاصدار غير موجود";
        }

        public ApplicationVersionWithCodeNotFoundException(string applicationVersionCode)
        {
            Code = (int)IdentityErrorCodes.ApplicationVersionWithCodeNotFound;
            MessageEn = $"Application version does not exist for code: {applicationVersionCode}!";
            MessageAr = $"code: {applicationVersionCode} هذا الاصدار غير موجود";
        }
    }
}
