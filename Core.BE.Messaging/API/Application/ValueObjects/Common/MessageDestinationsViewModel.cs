using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    [DataContract]
    public class MessageDestinationsViewModel
    {
        [DataMember]
        public DestinationType DestinationType { get; set; }

        [DataMember]
        public List<string> Recipients { get; set; }
    }
}
