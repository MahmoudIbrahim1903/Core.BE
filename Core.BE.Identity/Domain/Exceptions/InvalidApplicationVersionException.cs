using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class InvalidApplicationVersionException : BusinessException
    {
        public InvalidApplicationVersionException(string version)
        {
            Code = (int)IdentityErrorCodes.InvalidApplicationVersion;
            MessageEn = "No version found with number: " + version;
            MessageAr = "لا يوجد اصدار برقم: " + version;
        }
    }
}
