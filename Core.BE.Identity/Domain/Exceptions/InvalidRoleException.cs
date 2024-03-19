using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class InvalidRoleException : BusinessException
    {
        public InvalidRoleException()
        {
            Code = (int)IdentityErrorCodes.InvalidRole;
            Resourcekey = IdentityErrorCodes.InvalidRole.ToString();
        }
        public InvalidRoleException(string clientEn, string clientAr)
        {
            Code = (int)IdentityErrorCodes.InvalidRole;
            MessageEn = $"You are not supposed to login from {clientEn}";
            MessageAr = "لا يمكنك تسجيل الدخول عن طريق  " + clientAr;
        }
        
    }
}
