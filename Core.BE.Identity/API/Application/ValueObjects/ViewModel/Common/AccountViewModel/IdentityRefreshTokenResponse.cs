using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class IdentityRefreshTokenResponse : BaseIdentityTokenResponse
    {
        public IdentityRefreshTokenResponse(string accessToken, string tokenType,
            int expiresInSeconds, string refreshToken, string accessTokenType)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresInSeconds = expiresInSeconds;
            RefreshToken = refreshToken;
            AccessTokenType = accessTokenType;
           
        }
    }
}
