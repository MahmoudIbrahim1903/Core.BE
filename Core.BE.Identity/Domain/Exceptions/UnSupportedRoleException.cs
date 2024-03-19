using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UnSupportedRoleException: BusinessException
    {
        public UnSupportedRoleException(string countryName,string role)
        {
            Code = (int)IdentityErrorCodes.UnSupportedRole;
            MessageEn = $"{countryName} did not support this role: {role} !";
            MessageAr = countryName + "لا تدعم هذا الدور : " + role ;
        }
    }
}
