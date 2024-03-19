using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Exceptions
{
    public class MaxAudioSizeExceededException : BusinessException
    {
        public MaxAudioSizeExceededException(string maxAudioSize)
        {
            Code = (int)MediaErrorCodes.MaxAudioSizeExceeded;
            MessageEn = $"Audio size should not exceed {maxAudioSize} MegaBytes";
            MessageAr = $"يجب ألا يزيد حجم التسجبل الصوتي عن {maxAudioSize} ميجا";
        }
    }
}
