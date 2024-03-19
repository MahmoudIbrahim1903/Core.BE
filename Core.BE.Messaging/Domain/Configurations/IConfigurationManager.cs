using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Configurations
{
    public interface IConfigurationManager
    {
        int GetMessageDelayValue();
        string GetWhiteList();
        bool EnableWhiteList();
        string GetNotificationHubConnectionString();
        string GetNotificationHubPath();
        string GetIntermicroservicesHashKey();
        string GetFirebaseMobileCredentialsFilePath();
        string GetFirebaseWebCredentialsFilePath();
        bool GetTrackAdminNotifications();
        int GetNotificationsTrackingSLA();
    }
}
