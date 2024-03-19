using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    public class EndUserConversationDetailsCriteria
    {
        public List<Status> Statuses { get; set; }
        public Direction Direction { get; set; }
        public int? CurrentMessageId { get; set; }
        public string PeerId { get; set; }
    }
}
