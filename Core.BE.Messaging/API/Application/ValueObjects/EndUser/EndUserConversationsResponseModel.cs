using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    [DataContract]
    public class EndUserConversationsResponseModel
    {
        [DataMember]
        public string PeerId { get; set; }
        [DataMember]
        public List<ConversationMessageViewModel> Messages { get; set; }
    }
}
