using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    public class ServerPushLiveUpdateQMessage
    {
        public Payload Payload { get; set; }
        public Destination Destination { set; get; } 
    }

    public class Payload
    {
        public string Type { get; set; }
        public Dictionary<string, string> Data { set; get; }

    }

    public class Destination
    {
        public int Type { set; get; } //All = 0, Group = 1, Users = 2
        public string GroupName { get; set; } //if type is Group
        public List<string> UsersIds { set; get; } //if type is users

    }
}
