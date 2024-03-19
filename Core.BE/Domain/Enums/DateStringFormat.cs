using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Domain.Enums
{
    public enum DateStringFormat
    {
        DateTimeCompactFormat, //yyyyMMddHHmmss
        DateTime12HourDisplayFormat, //dd/MM/yyyy hh:mm tt
        DateTimeHighPrecisionFormat //yyyy-MM-dd'T'HH:mm:ss.SSSSSS
    }
}
