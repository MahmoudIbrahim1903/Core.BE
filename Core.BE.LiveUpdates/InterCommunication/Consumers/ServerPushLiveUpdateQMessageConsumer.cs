using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.LiveUpdates.Enums;
using Emeint.Core.BE.LiveUpdates.LiveUpdatesHub;
using Emeint.Core.BE.LiveUpdates.Services.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.LiveUpdates.InterCommunication.Consumers
{
    public class ServerPushLiveUpdateQMessageConsumer : IInterCommMessageConsumer<ServerPushLiveUpdateQMessage>
    {
        private readonly IServerLiveUpdatesService _serverLiveUpdatesService;
        public ServerPushLiveUpdateQMessageConsumer(IServerLiveUpdatesService serverLiveUpdatesService)
        {
            _serverLiveUpdatesService = serverLiveUpdatesService;
        }
        public async Task Consume(ConsumeContext<ServerPushLiveUpdateQMessage> context)
        {
            var message = context.Message;
            switch ((ServerPushLiveUpdateDestinationType)message.Destination.Type)
            {
                case ServerPushLiveUpdateDestinationType.All:
                    await _serverLiveUpdatesService.ServerPushLiveUpdateToAll(message.Payload);
                    break;
                case ServerPushLiveUpdateDestinationType.Group:
                    await _serverLiveUpdatesService.ServerPushLiveUpdateToGroup(message.Destination.GroupName, message.Payload);
                    break;
                case ServerPushLiveUpdateDestinationType.Users:
                    await _serverLiveUpdatesService.ServerPushLiveUpdateToUsersIds(message.Destination.UsersIds, message.Payload);
                    break;
            }
        }
    }
}
