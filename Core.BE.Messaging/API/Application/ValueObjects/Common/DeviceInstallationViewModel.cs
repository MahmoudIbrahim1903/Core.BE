using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class DeviceInstallationViewModel
    {
        public string UserId { get; set; }
        public string PushNotificationToken { get; set; }
        public string Platform { get; set; }
        public string DeviceId { get; set; }
        public string Language { get; set; }
        public List<string> Tags { get; set; }
    }
}
