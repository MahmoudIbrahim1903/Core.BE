using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    public class ChattingUserProfileChangeQMessage
    {
        public string ImageUrl { set; get; }
        public string UserId { set; get; }
        public string UserName { get; set; }
    }
}
