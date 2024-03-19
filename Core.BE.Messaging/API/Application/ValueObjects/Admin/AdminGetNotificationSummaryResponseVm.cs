using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    [DataContract]
    public class AdminGetNotificationSummaryResponseVm : BaseMessageResponseViewModel
    {
        [DataMember]
        public string MessageTypeCode { get; set; }
        [DataMember]
        public string SenderId { get; set; }
        [DataMember]
        public string MessageCategoryCode { get; set; }
        [DataMember]
        public string CreationDate { get; set; }
        [DataMember]
        public string CreatedBy{ get; set; }
        [DataMember]
        public string Notes { get; internal set; }
    }
}
