
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Emeint.Core.BE.Utilities
{
    public static class StringUtility
    {

        public static bool IsHTML(string body)
        {
            return Regex.IsMatch(body, "<(.|\n)*?>");
        }
        public static string TextToRtl(string text)
        {
            return ((char)0x200F).ToString() + text;
        }
        public static string TextToLtr(string text)
        {
            return ((char)0x200E).ToString() + text;
        }
        public static bool IsNumber(string value)
        {
            if (Regex.IsMatch(value, "[^0-9]"))
                return false;     // not number
            else
                return true;      // is number
        }
        public static string GenerateAlphaNumericString(int length)
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateRandomNumbersString(int Length)
        {
            Random Rand = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Length; i++)
                sb.Append(Rand.Next(0, 9));

            return sb.ToString();
        }
    }
}