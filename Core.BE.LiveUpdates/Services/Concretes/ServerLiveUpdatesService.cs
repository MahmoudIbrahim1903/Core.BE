using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.LiveUpdates.LiveUpdatesHub;
using Emeint.Core.BE.LiveUpdates.Services.Contracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.LiveUpdates.Services.Concretes
{
    public class ServerLiveUpdatesService : IServerLiveUpdatesService
    {
        private readonly IHubContext<BaseHub> _hubContext;
        private readonly ICacheService _cacheService;

        public ServerLiveUpdatesService(IHubContext<BaseHub> hubContext, ICacheService cacheService)
        {
            _hubContext = hubContext;
            _cacheService = cacheService;
        }

        public async Task ServerPushLiveUpdateToAll(Payload payload)
        {
            await _hubContext.Clients.All.SendAsync("server_push_live_update", payload);
        }

        public async Task ServerPushLiveUpdateToGroup(string groupName, Payload payload)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("server_push_live_update", payload);
        }

        public async Task ServerPushLiveUpdateToUsersIds(List<string> usersIds, Payload payload)
        {
            await _hubContext.Clients.Users(usersIds).SendAsync("server_push_live_update", payload);
        }
    }
}
