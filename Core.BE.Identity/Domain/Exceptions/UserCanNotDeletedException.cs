using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class UserCanNotDeletedException : BusinessException
    {
        public UserCanNotDeletedException(string reasonEn, string reasonAr)
        {
            Code = (int)IdentityErrorCodes.UserCanNotDeleted;
            MessageEn = $"User can not be deleted: {reasonEn}";
            MessageAr = $"لا يمكنك حذف المستخدم :{reasonAr}";
        }
    }
}
