using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UserAlreadyExistException : BusinessException
    {
        public UserAlreadyExistException(string parameter)
        {
            Code = (int)IdentityErrorCodes.UserAlreadyExists;
            Resourcekey = IdentityErrorCodes.UserAlreadyExists.ToString();
            MessageParameters.Add(parameter);
            MessageEn = $"this user already exists: {parameter}";
            MessageAr = $"هذا المستخدم موجود مسبقاً: {parameter}";
        }
    }
}