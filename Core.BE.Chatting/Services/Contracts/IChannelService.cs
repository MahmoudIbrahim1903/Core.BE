using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Chatting.Application.Dtos;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Services.Contracts
{
    public interface IChannelService
    {
        PagedList<GetChannelResponseDto> GetChannels(string memberId,string type, string contactName, PaginationVm pagination);
        GetChannelResponseDto GetChannel(string channelUrl);
    }
}
