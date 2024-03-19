using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    public class UpdateMessageStatusViewModel
    {
        public string MessageCode { get; set; }
        public MessageStatusByEnduser Status { get; set; }
        //public Status Status { get; set; }
    }
}