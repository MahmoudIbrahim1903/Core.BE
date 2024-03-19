using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    [DataContract]
    public class GetNotificationsResponse : Common.PaginationResponseViewModel
    {

        public GetNotificationsResponse()
        {
            Messages = new List<EndUserNotificationResponseModel>();
        }

        [DataMember]
        public List<EndUserNotificationResponseModel> Messages { get; set; }
    
    }
}
