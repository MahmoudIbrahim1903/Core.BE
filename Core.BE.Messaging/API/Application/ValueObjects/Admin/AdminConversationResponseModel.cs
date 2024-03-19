using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    [DataContract]
    public class AdminConversationResponseModel
    {
        [DataMember]
        public string FirstPeerId { get; set; }
        [DataMember]
        public string SecondPeerId { get; set; }
        public string PeerId { get; set; }
        [DataMember]
        public List<ConversationMessageViewModel> Messages { get; set; }
    }
}