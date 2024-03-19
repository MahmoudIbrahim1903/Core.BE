using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Exceptions
{
    public class UnsupportedAudioFileException : BusinessException
    {
        public UnsupportedAudioFileException(string fileExtension)
        {

            Code = (int)MediaErrorCodes.UnsupportedAudioFile;
            MessageEn = $"Unsupported audio file extension: {fileExtension}!";
            MessageAr = "لا يدعم النظام الملفات من النوع: " + fileExtension;
        }
    }
}
