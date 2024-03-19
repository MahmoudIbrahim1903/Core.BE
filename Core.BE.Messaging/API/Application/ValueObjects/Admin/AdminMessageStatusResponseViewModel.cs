using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    public class AdminMessageStatusResponseViewModel //: BaseMessageViewModel
    {
        //[DataMember]
        //public string Title { get; set; }
        //[DataMember]
        //public string Body { get; set; }
        //[DataMember]
        //public string MessageTypeCode { get; set; }
        //[DataMember]
        //public string SenderId { get; set; }
        //[DataMember]
        //public MessageSource MessageSource { get; set; }
        //[DataMember]
        //public Status Status { get; set; }
        //[DataMember]
        //public string MessageCategoryCode { get; set; }
        //[DataMember]
        //public string SentDate { get; set; }
        //[DataMember]
        //public string DeliveredDate { get; set; }
        //[DataMember]
        //public Dictionary<string, string> ExtraParams { get; set; }

        [DataMember]
        public string ToUserId { get; set; }
        [DataMember]
        public Status Status { get; set; }
        [DataMember]
        public string SentDate { get; set; }
        [DataMember]
        public string DeliveredDate { get; set; }
        [DataMember]
        public Domain.Enums.Platform Platform { get; set; }
    }
}
