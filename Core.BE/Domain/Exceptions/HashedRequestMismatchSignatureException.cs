using Emeint.Core.BE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class HashedRequestMismatchSignatureException : BusinessException
    {
        public HashedRequestMismatchSignatureException()
        {
            Code = (int)ErrorCodes.HashedValueMismatchSignature;
            MessageEn = string.Format("Hashed Request Mismatch Signature");
            MessageAr = string.Format("النتيجة المشفرة غير متطابقة مع التوقيع");
        }
    }
}