using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.SMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Exceptions
{
    public class MessageTemplateNotFoundException : BusinessException
    {
        public MessageTemplateNotFoundException(string templateCode)
        {
            Code = (int)SmsErrorCodes.TemplateNotFound;
            MessageEn = $"template not found: {templateCode } ";
            MessageAr = $"لا يوجد قالب بهذا الكود : {templateCode}";
        }
    }
}
