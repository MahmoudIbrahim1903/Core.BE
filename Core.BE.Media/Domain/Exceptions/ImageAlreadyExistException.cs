using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Enums;

namespace Emeint.Core.BE.Media.Domain.Exceptions
{
    public class ImageAlreadyExistException : BusinessException
    {
        public ImageAlreadyExistException(string crn)
        {
            Code = (int)MediaErrorCodes.ImageAlreadyExist;
            MessageEn = "Image already exists, crn: " + crn;
            MessageAr = "هذه الصورة موجودة بالفعل, crn: " + crn;
        }
    }
}