using Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Application.Dtos
{
    public class GetChannelResponseDto
    {
        public List<ChannelMemberDto> ChannelMembers { set; get; }
        public string ChannelUrl { get; set; }
    }
}
