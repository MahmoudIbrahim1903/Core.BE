using Emeint.Core.BE.InterCommunication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.LiveUpdates.Services.Contracts
{
    public interface IServerLiveUpdatesService
    {
        Task ServerPushLiveUpdateToAll(Payload payload);
        Task ServerPushLiveUpdateToGroup(string groupName, Payload payload);
        Task ServerPushLiveUpdateToUsersIds(List<string> usersIds, Payload payload);
    }
}
