using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Domain.Managers
{
    public class TextManager
    {
        public TextManager()
        {

        }
        public static string ReplaceMany(string text, Dictionary<string, string> inputParams)
        {
            //to DO Validations
            if (!string.IsNullOrEmpty(text) && inputParams != null)
            {
                foreach (var inputParam in inputParams)
                {
                    text = text.Replace($"{inputParam.Key}", inputParam.Value);
                }
            }
            return text;
        }
    }
}
