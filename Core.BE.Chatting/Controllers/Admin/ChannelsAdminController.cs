using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Chatting.Application.Dtos;
using Emeint.Core.BE.Chatting.Domain.Configurations;
using Emeint.Core.BE.Chatting.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Controllers.Admin
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/admin/chatting/channels/v{version:apiVersion}")]
    public class ChannelsAdminController : Controller
    {
        private readonly IChannelService _channelService;
        private readonly IConfigurationManager _configurationManager;
        public ChannelsAdminController(IChannelService channelService, IConfigurationManager configurationManager)
        {
            _channelService = channelService;
            _configurationManager = configurationManager;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, ChatFullController")]
        public Response<List<GetChannelResponseDto>> GetChannels(string type, string contact_name)
        {
            var channels = _channelService.GetChannels(_configurationManager.GetChattingAdminId(), type, contact_name, pagination: null);

            return new Response<List<GetChannelResponseDto>>
            {
                Data = channels.List
            };
        }

        [HttpGet("{channel_url}")]
        [Authorize(Roles = "SuperAdmin, ChatFullController")]

        public Response<GetChannelResponseDto> GetChannel([FromRoute]string channel_url)
        {
            var channel = _channelService.GetChannel(channel_url);
            channel.ChannelMembers = channel.ChannelMembers.Where(m => m.Id != _configurationManager.GetChattingAdminId()).ToList();
            return new Response<GetChannelResponseDto>
            {
                Data = channel
            };
        }
    }
}
