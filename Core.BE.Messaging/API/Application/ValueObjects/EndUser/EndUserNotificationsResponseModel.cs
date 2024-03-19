using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    [DataContract]
    public class EndUserNotificationResponseModel : BaseMessageResponseViewModel
    {
        [DataMember]
        public string MessageTypeCode { get; set; }

        [DataMember]
        public string SentDate { get; set; }

        [DataMember]
        public Dictionary<string, string> ExtraParams { get; set; }
    }
}
