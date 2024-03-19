using Emeint.Core.BE.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Emeint.Core.BE.Utilities
{
    public static class EmailUtility
    {
        public static void ValidateEmail(string email)
        {
            if (email == null)
                throw new InvalidEmailException("email");

            email = email.Trim();

            if (email == string.Empty)
                throw new InvalidEmailException(email);

            if (!IsValidEmail(email))
                throw new InvalidEmailException(email);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var emailPattern = "^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$";
            var emailRegex = new Regex(emailPattern, RegexOptions.None);
            Match emailMatch = emailRegex.Match(email);
            return emailMatch.Success;
        }
    }
}