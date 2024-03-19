using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    public class SendNotificationQMesssage
    {
        public bool Alert { get; set; }
        public List<MessageDestinationsViewModel> Destinations { get; set; }
        public int MessageSource { get; set; } // SystemEventMessage = 0, EndUserMessage = 1, AdminMessage = 2
        public string MessageTypeCode { get; set; }
        public List<string> TitleParams { get; set; }
        public List<string> DisplayParams { get; set; }
        public List<string> TitleParamsAr { get; set; }
        public List<string> DisplayParamsAr { get; set; }
        public List<string> TitleParamsSw { get; set; }
        public List<string> DisplayParamsSw { get; set; }
        public Dictionary<string, string> ExtraParams { get; set; }
        public bool SendAsPush { get; set; }
    }
    public class MessageDestinationsViewModel
    {
        [DataMember]
        public int DestinationType { get; set; } //0 User,1 Group, 2 All 

        [DataMember]
        public List<string> Recipients { get; set; }
    }

}