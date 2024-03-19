using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    [DataContract]
    public class SendNotificationRequestDto : SendNotificationBaseRequestDto
    {
        [DataMember]
        public string Code { get; set; }
        public MessageSource MessageSource { get; set; } // msg source
        public List<string> TitleParams { get; set; }
        public List<string> DisplayParams { get; set; }
        public List<string> TitleParamsAr { get; set; }
        public List<string> DisplayParamsAr { get; set; }
        public List<string> TitleParamsSw { get; set; }
        public List<string> DisplayParamsSw { get; set; }

        public string Notes { get; set; }

        public SendNotificationRequestDto()
        {
            TitleParams = new List<string>();
            TitleParamsAr = new List<string>();
            TitleParamsSw = new List<string>();
            DisplayParams = new List<string>();
            DisplayParamsAr = new List<string>();
            DisplayParamsSw = new List<string>();
            // the default is to send it as a push notification.
            // if false, message will only be saved in DB, and pulled from DB
            SendAsPush = true;
        }
    }
}
