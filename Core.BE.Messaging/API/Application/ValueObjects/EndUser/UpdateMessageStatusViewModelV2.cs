using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    public class UpdateMessageStatusViewModelV2
    {
        public int NotificationId { get; set; }
        public MessageStatusByEnduser Status { get; set; }
    }
}
