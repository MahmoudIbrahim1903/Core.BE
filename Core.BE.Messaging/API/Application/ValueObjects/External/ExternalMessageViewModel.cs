using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.External
{
    public class ExternalMessageViewModel
    {
        [DataMember]
        public MessageDestinationsViewModel Destinations { get; set; }
        [DataMember]
        public string MessageTypeCode { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Body { get; set; }
    }
}
