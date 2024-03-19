using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Chatting.Application.Dtos;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using Emeint.Core.BE.Chatting.Services.Contracts;

namespace Emeint.Core.BE.Chatting.Controllers.Server
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/chatting/server/channels/v{version:apiVersion}")]
    public class ChannelsServerController : Controller
    {
        private readonly IChannelService _channelService;
        public ChannelsServerController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpGet]
        public Response<List<GetChannelResponseDto>> GetChannels([FromQuery] string user_id)
        {
            var channels = _channelService.GetChannels(user_id, type: null, contactName: null, pagination: null);

            return new Response<List<GetChannelResponseDto>>
            {
                Data = channels.List
            };
        }
    }
}
