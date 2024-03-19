using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Infractructure.Model
{
    public class Channel : Entity
    {
        public string ChannelUrl { get; set; }
        public List<ChannelMember> ChannelMembers { set; get; }
    }
}
