using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.External;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;

namespace Emeint.Core.BE.Notifications.Domain.Manager
{
    public interface IHashingManager
    {
        string HashSendMessageRequest(SystemEventNotificationDto message);
        string HashSendMessageRequest(ExternalMessageViewModel serverVM);
    }
}
