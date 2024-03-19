using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    public class RegisterForPushViewModel
    {
        public string UserId { get; set; }
        public string PushNotificationToken { get; set; }
        public string DeviceId { get; set; }
        //public List<string> Tags { get; set; }
    }
}
