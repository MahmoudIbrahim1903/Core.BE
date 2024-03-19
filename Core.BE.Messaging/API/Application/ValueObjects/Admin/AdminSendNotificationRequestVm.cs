using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    [DataContract]
    public class AdminSendNotificationRequestVm : SendNotificationBaseRequestDto
    {
        [DataMember]
        public string ContentEn { get; set; }
        [DataMember]
        public string ContentAr { get; set; }
        [DataMember]
        public string ContentSw { get; set; }

        [DataMember]
        public string TitleEn { get; set; }
        [DataMember]
        public string TitleAr { get; set; }
        [DataMember]
        public string TitleSw { get; set; }
        [DataMember]
        public string Notes { get; set; }
    }
}
