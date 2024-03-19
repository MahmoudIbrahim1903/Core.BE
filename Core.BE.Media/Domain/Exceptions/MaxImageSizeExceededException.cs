using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Exceptions
{
    public class MaxImageSizeExceededException : BusinessException
    {
        public MaxImageSizeExceededException(string maxImageSize)
        {

            Code = (int)MediaErrorCodes.MaxImageSizeExceeded;
            MessageEn = $"Image size should not exceed {maxImageSize} KiloByte";
            MessageAr = $"يجب ألا يزيد حجم الصورة عن {maxImageSize} kiloByte";
        }
    }
}
