using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server
{
    public class SystemEventNotificationDto : SendNotificationBaseRequestDto // TODO: Review naming convention
    {
        [DataMember]
        public List<string> DisplayParams { get; set; }
        [DataMember]
        public List<string> TitleParams { get; set; }
        [DataMember]
        public List<string> DisplayParamsAr { get; set; }
        [DataMember]
        public List<string> TitleParamsAr { get; set; }
        [DataMember]
        public List<string> DisplayParamsSw { get; set; }
        [DataMember]
        public List<string> TitleParamsSw { get; set; }

    }
}
