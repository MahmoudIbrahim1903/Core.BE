using System.Threading.Tasks;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.Identity.API.Infrastructure.CustomGrant;
public class CustomTokenRevocationResponseGenerator : TokenRevocationResponseGenerator
{
    public CustomTokenRevocationResponseGenerator(
        IReferenceTokenStore referenceTokenStore,
        IRefreshTokenStore refreshTokenStore,
        ILogger<TokenRevocationResponseGenerator> logger)
        : base(referenceTokenStore, refreshTokenStore, logger)
    {
    }

    protected override async Task<bool> RevokeRefreshTokenAsync(TokenRevocationRequestValidationResult validationResult)
    {
        Logger.LogInformation("Revoking refresh token");

        // Assume the token is the access token instead of the refresh token
        var token = await ReferenceTokenStore.GetReferenceTokenAsync(validationResult.Token);

        if (token != null)
        {
            Logger.LogInformation($"Revoking refresh token for clientId {token.ClientId}");

            if (token.ClientId == validationResult.Client.ClientId)
            {
                Logger.LogDebug("Refresh token revoked");
                await RefreshTokenStore.RemoveRefreshTokenAsync(validationResult.Token);
                // await ReferenceTokenStore.RemoveReferenceTokensAsync(token.SubjectId, token.ClientId);
            }
            else
            {
                Logger.LogWarning("Client {clientId} denied from revoking a refresh token belonging to Client {tokenClientId}", validationResult.Client.ClientId, token.ClientId);
            }

            return true;
        }

        return false;
    }
}