using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    public class AdminUserResponseViewModel
    {
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public Domain.Enums.Platform Platform { get; set; }
        [DataMember]
        public string LanguageCode { get; set; }
        [DataMember]
        public string Name { get; set; }
        //[DataMember]
        //public string CreationDate { get; set; }
    }
}
