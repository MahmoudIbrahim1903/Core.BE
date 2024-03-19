using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UpdateUserFailedException : BusinessException
    {
        public UpdateUserFailedException(string moreDetails, string userId)
        {
            Code = (int)IdentityErrorCodes.UpdateUserFailedException;
            Resourcekey = IdentityErrorCodes.UpdateUserFailedException.ToString();
            MessageEn = "Update user failed!";
            MessageAr = "فشل تحديث بيانات المستخدم!!";
            MoreDetails = $"Failed to update user:{userId}, error details:{MoreDetails}";
        }
    }
}
