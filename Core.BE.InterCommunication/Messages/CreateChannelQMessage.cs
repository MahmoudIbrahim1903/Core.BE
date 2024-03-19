using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    public class CreateChannelQMessage
    {
        public List<CreateChatChannelMemberDto> ChannelMembers { set; get; }
    }
}
