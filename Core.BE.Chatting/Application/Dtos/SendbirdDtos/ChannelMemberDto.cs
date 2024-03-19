using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos
{
    public class ChannelMemberDto
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string Type { get; set; }
        public ImageViewModel Image { get; set; }
    }
}
