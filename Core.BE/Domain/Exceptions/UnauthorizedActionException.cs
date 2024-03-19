using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class UnauthorizedActionException : BusinessException
    {
        public UnauthorizedActionException() : this(null)
        {
            Code = (int)ErrorCodes.UnauthorizedAction;
            MessageEn = "You are not authorized to do this action";
            MessageAr = "لا توجد صلاحيات كافية لاتمام العملية";
        }

        public UnauthorizedActionException(string reason)
        {
            Code = (int)ErrorCodes.UnauthorizedAction;
            MessageEn = string.Format("You are not authorized to do this action, because: " + reason);
            MessageAr = "لا توجد صلاحيات كافية لاتمام العملية: ";
        }
    }
}