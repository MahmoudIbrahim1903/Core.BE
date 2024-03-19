using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    [DataContract]
    public class IdentityServerLogoutRequestDto
    {
        [DataMember]
        public string client_id { set; get; }
        [DataMember]
        public string client_secret { set; get; }
        [DataMember]
        public string token { set; get; }
        [DataMember]
        public string refresh_token { set; get; }
    }
}
