using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class CountryNotFoundException : BusinessException
    {
        public CountryNotFoundException(string countryCode)
        {
            Code = (int)IdentityErrorCodes.CountryNotFound;
            MessageEn = $"No country found with code {countryCode}";
            MessageAr = $"هذه الدولة ليست متاحة الآن {countryCode} ";
        }
    }
}