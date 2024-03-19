using Emeint.Core.BE.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Emeint.Core.BE.Utilities
{
    public static class NationalIdUtility
    {
        public static DateTime? CalculateDateOfBirthFromEgyptionNationalId(string nationalNumber)
        {
            string year = nationalNumber.Substring(1, 2);
            int month = Convert.ToInt32(nationalNumber.Substring(3, 2));
            int day = Convert.ToInt32(nationalNumber.Substring(5, 2));

            string yearCentury;
            var century = Convert.ToInt32(nationalNumber.Substring(0, 1));

            if (century == 2)
                yearCentury = "19";
            else
                yearCentury = "20";

            var completeYear = int.Parse(yearCentury + year);
            DateTime? dateOfBirth = null;

            try
            {
                dateOfBirth = new DateTime(completeYear, month, day);
            }
            catch (Exception ex)
            {
                dateOfBirth = null;
            }

            return dateOfBirth;
        }

        public static void Validate(string nationalId, string couontryCode)
        {
            if (couontryCode == "EGY")
            {
                string nationalIdRegix = "(2|3)[0-9][1-9][0-1][1-9][0-3][1-9](01|02|03|04|11|12|13|14|15|16|17|18|19|21|22|23|24|25|26|27|28|29|31|32|33|34|35|88)\\d\\d\\d\\d\\d";
                var nationalIdRgx = new Regex(nationalIdRegix, RegexOptions.None);
                Match egyptianNationalIdMatch = nationalIdRgx.Match(nationalId);
                if (!egyptianNationalIdMatch.Success)
                    throw new InvalidNationalIdException(nationalId);
            }
        }
    }
}
