using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Domain.Managers
{
    public class PrefixKeyVaultSecretManager : KeyVaultSecretManager
    {
        const string SHARED_KEY_PREFIX = "Shared-";
        private readonly string _prefix;

        public PrefixKeyVaultSecretManager(string prefix)
            => _prefix = $"{prefix}-";

        public override bool Load(SecretProperties properties)
            => properties.Name.StartsWith(_prefix) || properties.Name.StartsWith(SHARED_KEY_PREFIX);

        public override string GetKey(KeyVaultSecret secret)
        {
            if (secret.Name.StartsWith(_prefix))
            {
                return secret.Name[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
            }
            else if (secret.Name.StartsWith(SHARED_KEY_PREFIX))
            {
                return secret.Name[SHARED_KEY_PREFIX.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
            }

            return secret.Name;
        }

    }
}
