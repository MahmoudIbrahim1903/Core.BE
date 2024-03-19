using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    public class EndUserNotificationResponseModelV2 : BaseMessageResponseViewModelV2
    {
        [DataMember]
        public string TypeCode { get; set; }
        //[DataMember]
        //public string SenderId { get; set; }
        //[DataMember]
        //public MessageSource MessageSource { get; set; }
        [DataMember]
        public MessageStatusByEnduser Status { get; set; }
        [DataMember]
        public string SentDate { get; set; }
        [DataMember]
        public string DeliveredDate { get; set; }
        [DataMember]
        public Dictionary<string, string> ExtraParams { get; set; }
    }
}
