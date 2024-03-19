using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Infractructure.Model
{
    public class User
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string Type { set; get; }
        public string ImageUrl { set; get; }
        public virtual List<ChannelMember> ChannelMembers { set; get; }
    }
}
