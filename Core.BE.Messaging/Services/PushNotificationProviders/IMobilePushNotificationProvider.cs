using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Services.PushNotificationProviders
{
    public interface IMobilePushNotificationProvider
    {
        Task<PushMessageResponse> PushMessage(PushNotificationDto pushMessageViewModel);
        Task<bool> CreateOrUpdatePushNotificationRegistration(DeviceInstallationViewModel installation);
        Task<bool> DeleteRegistration(DeviceInstallationViewModel installation);
        //TODO: register on tag 
    }
}
