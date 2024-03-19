using Emeint.Core.BE.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Emeint.Core.BE.Utilities
{
    public static class PhoneNumberUtility
    {

        private static readonly string egyptianPhoneNumberPatternWithDialCode = "^\\+201[0|1|2|5]{1}[0-9]{8}";
        private static readonly string egyptianPhoneNumberPatternWithOutDialCode = "^01[0|1|2|5]{1}[0-9]{8}";
        private static readonly string tanzaniaPhoneNumberPatternWithDialCode = "^\\+255[6|7]{1}[0-9]{8}";
        private static readonly string tanzaniaPhoneNumberWithOutDialCode = "^[6|7]{1}[0-9]{8}";
        //private static readonly string uaePhoneNumberPattern = "^(?:\\+971|0(0971)?)(?:[234679]|5[01256])\\d{7}$";
        public static string RemoveInvalidPhoneNumberChars(string phone) => string.IsNullOrEmpty(phone) 
            ? string.Empty 
            : phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        public static string ValidateAndRemoveCountryDialCode(string mobileNumber, string countryDialCode, int countryMobileNumberLength, string countryCode)
        {
            if (string.IsNullOrEmpty(mobileNumber))
                throw new InvalidPhoneNumberException(mobileNumber);

            mobileNumber = mobileNumber.Trim();
         
            mobileNumber = RemoveInvalidPhoneNumberChars(mobileNumber);

            if (mobileNumber.StartsWith("+"))
            {
                mobileNumber = mobileNumber.Substring(1);
            }
            else if (mobileNumber.StartsWith("00"))
            {
                mobileNumber = mobileNumber.Substring(2);
            }

            if (countryCode == "EGY")
            {
                if (mobileNumber.StartsWith(countryDialCode))
                {
                    mobileNumber = mobileNumber.Substring(countryDialCode.Length - 1);
                }
            }

            if (countryCode == "TZA")
            {
                if (mobileNumber.StartsWith(countryDialCode))
                {
                    mobileNumber = mobileNumber.Substring(countryDialCode.Length);
                }
            }
            
            if (mobileNumber.Length != countryMobileNumberLength)
                throw new InvalidPhoneNumberException(mobileNumber);

            switch (countryCode)
            {
                case "EGY":
                    var egyptianPhoneNumberRegex = new Regex(egyptianPhoneNumberPatternWithOutDialCode, RegexOptions.None);
                    Match egyptianPhoneMatch = egyptianPhoneNumberRegex.Match(mobileNumber);
                    if (!egyptianPhoneMatch.Success)
                        throw new InvalidPhoneNumberException(mobileNumber);
                    break;
                case "TZA":
                    var tanzaniaPhoneNumberRegex = new Regex(tanzaniaPhoneNumberWithOutDialCode, RegexOptions.None);
                    Match tanzaniaPhoneMatch = tanzaniaPhoneNumberRegex.Match(mobileNumber);
                    if (!tanzaniaPhoneMatch.Success)
                        throw new InvalidPhoneNumberException(mobileNumber);
                    break;
                //case "ARE":
                //    var uaePhoneNumberRegex = new Regex(uaePhoneNumberPattern, RegexOptions.None);
                //    Match uaePhoneMatch = uaePhoneNumberRegex.Match(mobileNumber);
                //    if (!uaePhoneMatch.Success)
                //        throw new InvalidPhoneNumberException(mobileNumber);
                //    break;
                default:
                    break;
            }


            return mobileNumber;
        }

        public static string ValidateAndAddCountryDialCode(string mobileNumber, string countryDialCode, int countryMobileNumberLength, string countryCode)
        {
            string mobileNumberNoCountryCode = mobileNumber;
            if (string.IsNullOrEmpty(mobileNumber.Trim()))
                throw new InvalidPhoneNumberException(mobileNumber);

            mobileNumber = RemoveInvalidPhoneNumberChars(mobileNumber);

            if (mobileNumber.StartsWith("00"))
            {
                mobileNumber = mobileNumber.Substring(2);
            }

            // To be refactored
            if (countryCode == "EGY")
                countryDialCode = countryDialCode.Substring(0, countryDialCode.Length - 1);

            if (countryCode == "TZA")
                countryDialCode = countryDialCode.Substring(0, countryDialCode.Length);

            if (!mobileNumber.StartsWith("+"))
            {
                if (mobileNumber.StartsWith(countryDialCode))
                    mobileNumber = $"+{mobileNumber}";
                else
                    mobileNumber = $"+{countryDialCode}{mobileNumber}";
            }
            
            if (countryCode == "EGY")
            { 
                if ((mobileNumber.Length - countryDialCode.Length - 1) != countryMobileNumberLength)
                    throw new InvalidPhoneNumberException(mobileNumberNoCountryCode);

            }

            if (countryCode == "TZA")
            { 
                if ((mobileNumber.Length - countryDialCode.Length) - 1 != countryMobileNumberLength)
                    throw new InvalidPhoneNumberException(mobileNumberNoCountryCode);
            }

            switch (countryCode)
            {
                case "EGY":
                    if (!IsValidEgyptionPhoneNumber(mobileNumber))
                        throw new InvalidPhoneNumberException(mobileNumberNoCountryCode);
                    break;
                case "TZA":
                    if(!IsValidTanzaniaPhoneNumber(mobileNumber))
                        throw new InvalidPhoneNumberException(mobileNumberNoCountryCode);
                    break;
                //case "ARE":
                //    var uaePhoneNumberRegex = new Regex(uaePhoneNumberPattern, RegexOptions.None);
                //    Match uaePhoneMatch = uaePhoneNumberRegex.Match(mobileNumber);
                //    if (!uaePhoneMatch.Success)
                //        throw new InvalidPhoneNumberException(mobileNumber);
                //    break;
                default:
                    break;
            }
            return mobileNumber;
        }


        public static bool IsValidEgyptionPhoneNumber(string mobileNumber)
        {
            var egyptianPhoneNumberRegex = new Regex(egyptianPhoneNumberPatternWithDialCode, RegexOptions.None);
            Match egyptianPhoneMatch = egyptianPhoneNumberRegex.Match(mobileNumber);
            return egyptianPhoneMatch.Success;
        }

        public static bool IsValidTanzaniaPhoneNumber(string mobileNumber)
        {
            var tanzaniaPhoneNumberRegex = new Regex(tanzaniaPhoneNumberPatternWithDialCode, RegexOptions.None);
            Match tanzaniaPhoneMatch = tanzaniaPhoneNumberRegex.Match(mobileNumber);
            return tanzaniaPhoneMatch.Success;
        }
    }
}