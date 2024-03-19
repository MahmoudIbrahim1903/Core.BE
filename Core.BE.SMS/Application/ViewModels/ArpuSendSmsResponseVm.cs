using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Application.ViewModels
{
    public class ArpuSendSmsResponseVm
    {
        public int MessageId { set; get; }
        public bool Status { set; get; }
        public string StatusDescription { set; get; }
        public DateTime TimeStamp { set; get; }
        public int MessageParts { set; get; }
    }
}
