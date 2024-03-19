using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Exceptions
{
    public class MaxVideoSizeExceededException : BusinessException
    {
        public MaxVideoSizeExceededException(string maxVideoSize)
        {

            Code = (int)MediaErrorCodes.MaxVideoSizeExceeded;
            MessageEn = $"Video size should not exceed {maxVideoSize} MegaBytes";
            MessageAr = $"يجب ألا يزيد حجم الفيديو عن {maxVideoSize} MegaBytes";
        }
    }
}