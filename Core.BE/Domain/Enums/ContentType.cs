using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Emeint.Core.BE.Domain.Enums
{
    public enum ContentType
    {
        [Description("application/json")]
        Json = 0,
        [Description("application/x-www-form-urlencoded")]
        Form = 1,
        [Description("multipart/form-data")]
        Multipart = 2
    }
}
