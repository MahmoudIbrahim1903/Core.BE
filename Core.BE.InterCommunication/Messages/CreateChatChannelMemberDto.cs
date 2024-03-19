using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    public class CreateChatChannelMemberDto
    {
        public string MemberId { set; get; }
        public string MemberName { set; get; }
        public string MemberType { set; get; }
        public string MemberImageUrl { set; get; }
    }
}
