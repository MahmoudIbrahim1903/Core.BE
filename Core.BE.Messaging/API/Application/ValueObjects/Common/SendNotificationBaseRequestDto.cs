using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    [DataContract]
    public class SendNotificationBaseRequestDto
    {
        [DataMember]
        public bool Alert { get; set; }
        [DataMember]
        public List<MessageDestinationsViewModel> Destinations { get; set; }
        [DataMember]
        public bool SendAsPush { get; set; }
        [DataMember]
        public Dictionary<string, string> ExtraParams { get; set; }
        [DataMember]
        public string MessageTypeCode { get; set; }

    }
}
