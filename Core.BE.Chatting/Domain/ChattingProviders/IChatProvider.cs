using Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Domain.ChattingProviders
{
    public interface IChatProvider
    {
        Task CreateChannel(List<ChannelMemberDto> channelMembers);
        Task DeleteChannel(string channelId);
        Task RegisterUser(string userId, string name, string type, string imageUrl);
        Task DeleteUser(string userId);

    }
}
