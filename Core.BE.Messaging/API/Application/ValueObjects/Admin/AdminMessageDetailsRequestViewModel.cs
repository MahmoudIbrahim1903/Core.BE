using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    [DataContract]
    public class AdminMessageDetailsRequestViewModel
    {
        //[DataMember]
        //public string ToUserId { get; set; }
        [DataMember]
        public string MessageCode { get; set; }
    }
}
