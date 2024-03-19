using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Infractructure.IRepositories
{
    public interface IChannelRepository
    {
        void CreateChannel(List<ChannelMember> channelMembers, string channelUrl);
        Channel GetChannel(List<ChannelMember> channelMembers);
        PagedList<Channel> GetChannels(string memberId, string type, string contactName, PaginationVm pagination);
        Channel GetChannel(string channelUrl);
    }
}
