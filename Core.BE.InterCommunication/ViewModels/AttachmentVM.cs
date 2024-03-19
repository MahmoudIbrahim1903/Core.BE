using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.ViewModels
{
    [DataContract]
    public class AttachmentVM
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FileBase64 { get; set; }
    }
}
