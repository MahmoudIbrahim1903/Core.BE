using Emeint.Core.BE.InterCommunication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.LiveUpdates.Services.Contracts
{
    public interface IGroupsService
    {
        Task<Payload> GetGroupUsersCountPayload(bool userJoined, string groupName, string userId);
    }
}
