using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs
{
    public class UserDataDTO 
    {
        public string ApplicationUserId { get; set; }

        public string Name { get; set; }

        public int UserDeviceId { get; set; }

        public string DeviceId { get; set; }

        public string LanguageCode { get; set; }

        public string PushNotificationToken { get; set; }

        public int ClientApplicationVersionId { get; set; }

        public Domain.Enums.Platform Platform { get; set; }

        public UserDataDTO(User user) 
        {
            ApplicationUserId = user.ApplicationUserId;
            Name = user.Name;
            UserDeviceId = user.UserDevice != null ? user.UserDevice.Id : 0;
            DeviceId = user.DeviceId;
            LanguageCode = user.LanguageCode;
            PushNotificationToken = user.PushNotificationToken;
            ClientApplicationVersionId = user.ClientApplicationVersion != null ? user.ClientApplicationVersion.Id : 0;
            Platform = user.DevicePlatform;
        }
    }
}
