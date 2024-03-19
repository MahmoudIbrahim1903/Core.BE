using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Chatting.Application.Dtos;
using Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos;
using Emeint.Core.BE.Chatting.Domain.ChattingProviders;
using Emeint.Core.BE.Chatting.Domain.Configurations;
using Emeint.Core.BE.Chatting.Infractructure.IRepositories;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using Emeint.Core.BE.Chatting.Services.Contracts;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Services.Concretes
{
    public class ChannelService : IChannelService
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IChatProvider _chatProvider;
        private readonly IConfigurationManager _configurationManager;

        public ChannelService(IChannelRepository channelRepository, IChatProvider chatProvider, IConfigurationManager configurationManager)
        {
            _channelRepository = channelRepository;
            _chatProvider = chatProvider;
            _configurationManager = configurationManager;
        }

        public GetChannelResponseDto GetChannel(string channelUrl)
        {
            var channel = _channelRepository.GetChannel(channelUrl);
            return new GetChannelResponseDto
            {
                ChannelUrl = channel.ChannelUrl,
                ChannelMembers = channel.ChannelMembers.Select(cm => new ChannelMemberDto
                {
                    Id = cm.UserId,
                    Image = new ImageViewModel { Url = cm.User.ImageUrl, ImageCode = null },
                    Name = cm.User.Name,
                    Type = cm.User.Type
                }).ToList()
            };
        }

        public PagedList<GetChannelResponseDto> GetChannels(string memberId, string type, string contactName, PaginationVm pagination)
        {
            var channels = _channelRepository.GetChannels(memberId, type, contactName, pagination);
            PagedList<GetChannelResponseDto> mappedChannels = new PagedList<GetChannelResponseDto>();  //TO DO: using automapper

            if (channels != null && channels.List != null && channels.List.Count() > 0)
            {
                mappedChannels = new PagedList<GetChannelResponseDto>(channels.TotalCount, pagination);
                mappedChannels.List = new List<GetChannelResponseDto>();

                foreach (var channel in channels.List)
                {
                    mappedChannels.List.Add(new GetChannelResponseDto()
                    {
                        ChannelUrl = channel.ChannelUrl,
                        ChannelMembers = channel.ChannelMembers.Where(m => m.UserId != memberId)
                            .Select(cm => new ChannelMemberDto
                            {
                                Id = cm.UserId,
                                Name = cm.User.Name,
                                Type = cm.User.Type,
                                Image = new ImageViewModel { Url = cm.User.ImageUrl }
                            }).ToList()
                    });
                }
            }
            return mappedChannels;
        }
    }
}
