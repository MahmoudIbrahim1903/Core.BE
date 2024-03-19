using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class ConversationMessageViewModel
    {
        public string SenderId { get; set; }
        public string MessageBody { get; set; }
        public string DeliveredDate { get; set; }
    }
}
