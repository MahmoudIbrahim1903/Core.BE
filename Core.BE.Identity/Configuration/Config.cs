using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Emeint.Core.BE.Identity.Configuration
{
    public class Config : IIdentityDataConfig
    {
        protected readonly IConfiguration _configuration;
        public Config(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // ApiResources define the apis in your system
        public virtual IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("identity", "Identity Service"),
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public virtual IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                //new IdentityResources.OpenId(),
                //new IdentityResources.Profile()
            };
        }

        // client want to access resources (aka scopes)
        public virtual IEnumerable<Client> GetClients()
        {

            int accessTokenLifeTime = int.TryParse(_configuration["AccessTokenLifeTime"], out int result) ? result : 3600; // 60 minutes
            int absoluteRefreshTokenLifetime = int.TryParse(_configuration["AbsoluteRefreshTokenLifetime"], out int absoluteRefreshTokenLifetimeResult) ? absoluteRefreshTokenLifetimeResult : 86400; // 24 hour
            int slidingRefreshTokenLifetime = int.TryParse(_configuration["SlidingRefreshTokenLifetime"], out int slidingRefreshTokenLifetimeResult) ? slidingRefreshTokenLifetimeResult : 3600; // 1 hour

            return new List<Client>
            {
                new Client
                {
                    ClientId = "admin",
                    ClientName = "Admin Portal",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret(this._configuration["ApisSecret"].Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "identity"
                    },
                    //Allow requesting refresh tokens for long lived API access
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = accessTokenLifeTime,
                    IdentityTokenLifetime = 2592000,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = absoluteRefreshTokenLifetime,
                    SlidingRefreshTokenLifetime = slidingRefreshTokenLifetime,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    //RedirectUris ={"-"}
                },

                new Client
                {
                    ClientId = "Xamarin",
                    ClientName = "Mobile",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,                    
                    //Used to retrieve the access token on the back channel.
                    ClientSecrets =
                    {
                        new Secret(this._configuration["ApisSecret"].Sha256())
                    },
                    RequireConsent = false,
                    AllowedScopes = new List<string>
                    {
                        "identity"
                    },
                    //Allow requesting refresh tokens for long lived API access
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = accessTokenLifeTime,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = absoluteRefreshTokenLifetime,
                    SlidingRefreshTokenLifetime = slidingRefreshTokenLifetime,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    IdentityTokenLifetime = 2592000
                }
            };
        }
    }
}