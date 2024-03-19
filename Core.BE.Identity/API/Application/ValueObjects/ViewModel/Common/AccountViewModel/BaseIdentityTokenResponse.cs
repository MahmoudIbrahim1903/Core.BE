using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    [DataContract]
    public class BaseIdentityTokenResponse
    {
        [DataMember]
        public string AccessToken { set; get; }
        [DataMember]
        public string TokenType { set; get; }
        [DataMember]
        public int ExpiresInSeconds { set; get; }
        [DataMember]
        public string RefreshToken { set; get; }
        [DataMember]
        public string AccessTokenType { set; get; }
    }
}
