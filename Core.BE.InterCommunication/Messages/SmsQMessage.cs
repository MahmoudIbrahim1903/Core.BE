using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    public class SmsQMessage
    {
        public int SmsProvider { set; get; } //0 ezagel, 1 victorylink, 2 twilio, 3 ARPU

        public List<string> PhoneNumbers { set; get; }

        public string TemplateCode { set; get; }

        public Dictionary<string, string> MessageParameters { set; get; }

        public string Language { set; get; }

        public string CreatedBy { set; get; }
    }
}
