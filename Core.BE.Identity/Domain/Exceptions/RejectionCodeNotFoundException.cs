
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class RejectionCodeNotFoundException : BusinessException
    {
        public RejectionCodeNotFoundException()
        {
            Code = (int)IdentityErrorCodes.UserNotFound;
            MessageEn = "No rejection reason found for the given code!";
            MessageAr = "لا يوجد سبب رفض للكود المعطى !";
        }
    }
}