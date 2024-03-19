using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class ApplicationVersionWithNumberNotFoundException : BusinessException
    {
        public ApplicationVersionWithNumberNotFoundException(string version)
        {
            Code = (int)IdentityErrorCodes.ApplicationVersionWithNumberNotFound;
            MessageEn = "No version found with number: " + version;
            MessageAr = "لا يوجد اصدار برقم: " + version;
        }
    }
}
