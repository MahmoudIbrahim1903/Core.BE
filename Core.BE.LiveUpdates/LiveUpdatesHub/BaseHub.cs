using CacheManager.Core.Logging;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.LiveUpdates.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.LiveUpdates.LiveUpdatesHub
{
    public class BaseHub : Hub
    {
        private readonly IGroupsService _groupsService;
        private readonly IConfiguration _configuration;
        private readonly IServerLiveUpdatesService _serverLiveUpdatesService;
        private readonly ILogger<BaseHub> _logger;

        public BaseHub(IGroupsService groupsService, IConfiguration configuration, IServerLiveUpdatesService serverLiveUpdatesService, ILogger<BaseHub> logger)
        {
            _configuration = configuration;
            _serverLiveUpdatesService = serverLiveUpdatesService;
            _groupsService = groupsService;
            _logger = logger;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            return base.OnDisconnectedAsync(ex);
        }

        [Authorize]
        public async Task join_group(string group_name)
        {
            _logger.LogInformation($"user before join: {Context.UserIdentifier}, group:{group_name}");

            await Groups.AddToGroupAsync(Context.ConnectionId, group_name);

            _logger.LogInformation($"user after join: {Context.UserIdentifier}, group:{group_name}");

            if (_configuration["ServerPushGroupUsersCount"] == true.ToString())
            {
                //get role to count
                var userRoles = Context.User.Claims.FirstOrDefault(c => c.Type == "role").Value.Split(',').ToList();
                var roles = _configuration["ServerPushGroupCountForUserRoles"].Split(',').ToList();

                Payload countPayload = null;
                if (userRoles.Intersect(roles).Any())
                {
                    countPayload = await _groupsService.GetGroupUsersCountPayload(true, group_name, Context.UserIdentifier);
                }
                else
                {
                    countPayload = await _groupsService.GetGroupUsersCountPayload(false, group_name, Context.UserIdentifier);
                }

                //wait for user to join group
                var waitDuration = _configuration["WaitForUserToJoinGroupDurationInSeconds"];

                if (string.IsNullOrEmpty(waitDuration))
                    waitDuration = "1";

                Thread.Sleep(TimeSpan.FromSeconds(Convert.ToInt32(waitDuration)));

                await _serverLiveUpdatesService.ServerPushLiveUpdateToGroup(group_name, countPayload);
                _logger.LogInformation($"user counted: {Context.UserIdentifier}, count:{countPayload.Data["count"]}, group:{group_name}");
            }
        }

        [Authorize]
        public async Task leave_group(string group_name)
        {
            _logger.LogInformation($"user before leave: {Context.UserIdentifier}, group:{group_name}");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group_name);
            _logger.LogInformation($"user after leave: {Context.UserIdentifier}, group:{group_name}");

            if (_configuration["ServerPushGroupUsersCount"] == true.ToString())
            {
                //get role to count down
                var userRoles = Context.User.Claims.FirstOrDefault(c => c.Type == "role").Value.Split(',').ToList();
                var roles = _configuration["ServerPushGroupCountForUserRoles"].Split(',').ToList();

                if (userRoles.Intersect(roles).Any())
                {
                    var countPayload = await _groupsService.GetGroupUsersCountPayload(false, group_name, Context.UserIdentifier);
                    await _serverLiveUpdatesService.ServerPushLiveUpdateToGroup(group_name, countPayload);
                    _logger.LogInformation($"user left: {Context.UserIdentifier}, count:{countPayload.Data["count"]}, group:{group_name}");
                }
            }
        }
    }
}
