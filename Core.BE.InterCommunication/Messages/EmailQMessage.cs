using Emeint.Core.BE.InterCommunication.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    [DataContract]
    public class EmailQMessage
    {
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string[] To { get; set; }
        [DataMember]
        public string[] Cc { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public List<AttachmentVM> Attachments { get; set; }  // file name and file hint file is base 64 
        [DataMember]
        public bool? IsImportant { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public Dictionary<string, string> ExtraParams { get; set; }
        [DataMember]
        public string FromUserId { get; set; }

        [DataMember]
        public string FromDisplayName { get; set; }
    }
}
