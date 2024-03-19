using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos
{
    public class CreateSendbirdChannelRequestDto
    {
        public string Name { get; set; }
        public string ChannelUrl { get; set; }
        public List<string> UserIds { get; set; }
    }
}
