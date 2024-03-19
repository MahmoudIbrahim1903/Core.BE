using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Enums
{
    /// <summary>
    /// From 700 to 799
    /// </summary>
    public enum SmsErrorCodes
    {
        SendSmsFailed = 700,
        EmptyMessage = 701,
        TemplateNotFound = 702,
        NoPhoneNumberProvided = 703
    }
}
