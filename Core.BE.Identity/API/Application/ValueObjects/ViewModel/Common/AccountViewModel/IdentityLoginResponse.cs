using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    [DataContract]
    public class IdentityLoginResponse : BaseIdentityTokenResponse
    {
        public IdentityLoginResponse(string accessToken, string tokenType,
            int expiresInSeconds, string refreshToken, string accessTokenType,
            string userId, string userDisplayName, List<string> allowedRoles,
            Dictionary<string, string> claims, bool resetPasswordRequird, string email, string phoneNumber
            )
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresInSeconds = expiresInSeconds;
            RefreshToken = refreshToken;
            AccessTokenType = accessTokenType;
            Claims = claims;
            UserId = userId;
            UserDisplayName = userDisplayName;
            AllowedRoles = allowedRoles;
            ResetPasswordRequired = resetPasswordRequird;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        [DataMember]
        public string UserDisplayName { set; get; }
        [DataMember]
        public string PhoneNumber { set; get; }
        [DataMember]
        public string Email { set; get; }
        [DataMember]
        public string UserId { set; get; }
        [DataMember]
        public List<string> AllowedRoles { set; get; }
        [DataMember]
        public bool ResetPasswordRequired { set; get; }
        [DataMember]
        public Dictionary<string, string> Claims { set; get; }

    }
}
