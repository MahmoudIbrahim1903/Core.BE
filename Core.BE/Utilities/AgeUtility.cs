using Emeint.Core.BE.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Utilities
{
    public static class AgeUtility
    {
        public static bool IsValidAge(DateTime birthDate, int? expectedMinimumAge = null, int? expectedMaximumAge = null)
        {
            int age = DateTime.UtcNow.Year - birthDate.Year;

            int minimumAge = 0, maximumAge = 150;

            if (expectedMinimumAge != null && expectedMinimumAge >= 0)
                minimumAge = expectedMinimumAge.Value;

            if (expectedMaximumAge != null && expectedMaximumAge <= 150)
                maximumAge = expectedMaximumAge.Value;

            if (age >= minimumAge && age <= maximumAge)
                return true;
            else
                return false;
        }

        public static int CalculateAge(DateTime dateOfBirth)
        {
            if (!IsValidAge(dateOfBirth))
                throw new Domain.Exceptions.InvalidOperationException("date_of_birth can't be a future date!", "تاريخ الميلاد يجب أن يكون في الماضي!");

            int age = DateTime.UtcNow.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }
    }
}
