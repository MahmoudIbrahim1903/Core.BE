using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.LiveUpdates.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.LiveUpdates.Services.Concretes
{
    public class GroupsService : IGroupsService
    {
        private readonly ICacheService _cacheService;
        public GroupsService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<Payload> GetGroupUsersCountPayload(bool userJoined, string groupName, string userId)
        {
            var message = new Payload();
            message.Type = "GroupUsersCountUpdated";

            var data = new Dictionary<string, string>();
            int count = 0;
            List<string> groupUsers = new List<string>();

            groupUsers = _cacheService.GetItemFromCache<List<string>>(groupName);

            if (groupUsers == null)
                groupUsers = new List<string>();

            switch (userJoined)
            {
                case true:
                    if (!groupUsers.Any(u => u == userId))
                    {
                        groupUsers.Add(userId);
                        _cacheService.SaveItemToCache<List<string>>(groupName, groupUsers);
                    }
                    count = groupUsers.Count();
                    break;

                case false:
                    if (groupUsers.Any(u => u == userId))
                        groupUsers.Remove(userId);

                    if (groupUsers.Count == 0)
                        _cacheService.DeleteItemFromCache(groupName);
                    else
                    {
                        _cacheService.SaveItemToCache<List<string>>(groupName, groupUsers);
                    }
                    count = groupUsers.Count();
                    break;
            }

            data.Add("count", count.ToString());
            message.Data = data;
            return message;
        }

    }
}
