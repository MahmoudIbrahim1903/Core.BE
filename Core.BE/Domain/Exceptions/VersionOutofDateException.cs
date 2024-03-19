using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Domain.Exceptions
{

    public class VersionOutofDateException : BusinessException
    {
        public VersionOutofDateException()
        {
            Code = (int)ErrorCodes.ForceVersionUpdate;
            MessageEn = $"Version is out of date, Please update your application";
            MessageAr = $"من فضلك قم يتحديث التطبيق";
        }
    }
}
