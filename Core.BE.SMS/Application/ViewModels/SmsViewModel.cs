using Emeint.Core.BE.SMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Application.ViewModels
{
    [DataContract]
    public class SmsViewModel
    {
        [DataMember]
        public SmsProvider SmsProvider { set; get; }

        [DataMember]
        public List<string> PhoneNumbers { set; get; }
       
        [DataMember]
        public string TemplateCode { set; get; }

        [DataMember]
        public Dictionary<string, string> MessageParameters { set; get; }

        [DataMember]
        public string Language { set; get; }

        [DataMember]
        public string CreatedBy { set; get; }
    }
}
