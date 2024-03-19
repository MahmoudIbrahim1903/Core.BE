using Emeint.Core.BE.InterCommunication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.API.Application.ValueObjects.ViewModels
{
    public class SendMailViewModel
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
        public List<AttachmentVM> Attachments { get; set; }
        [DataMember]
        public bool? IsImportant { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public Dictionary<string, string> ExtraParams { get; set; }
        [DataMember]
        public string FromUserId { get; set; }
    }
}
