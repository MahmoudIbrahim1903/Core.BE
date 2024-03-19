using Emeint.Core.BE.Media.API.Application.ValueObject.Dtos;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Enums;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public class AzureMediaServicesManager : IAzureMediaServicesManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureMediaServicesManager> _logger;
        public AzureMediaServicesManager(IConfiguration configuration, ILogger<AzureMediaServicesManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<MediaServicesEncodingJobStatus> StartVideoEncodingJobAsync(string videoUrl, string videoCode)
        {

            string transformName = _configuration["AzureMediaServices:TransformName"];
            string jobName = $"job-{videoCode}";
            string outputAssetName = $"output-{videoCode}";

            AzureMediaServicesClient client = await GetAzureMediaServicesClientAsync();

            await CreateTransformIfNotExistAsync(transformName, client);

            Job job = await CreateEncodingJobAsync(videoUrl, videoCode, transformName, jobName, outputAssetName, client);

            return (MediaServicesEncodingJobStatus)Enum.Parse(typeof(MediaServicesEncodingJobStatus), job.State.ToString());
        }

        public async Task<Job> CreateEncodingJobAsync(string videoUrl, string videoCode, string transformName, string jobName, string outputAssetName, AzureMediaServicesClient client)
        {
            string resourceGroupName = _configuration["AzureMediaServices:ResourceGroupName"];
            string accountName = _configuration["AzureMediaServices:AccountName"];

            //create output asset
            await client.Assets.CreateOrUpdateAsync(resourceGroupName, accountName, outputAssetName, new Asset(name: videoCode));

            JobInputHttp jobInput = new JobInputHttp(files: new[] { videoUrl });

            JobOutput[] jobOutputs = { new JobOutputAsset(outputAssetName) };

            Job job = await client.Jobs.CreateAsync(resourceGroupName, accountName, transformName, jobName,
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                });
            return job;
        }

        public async Task CreateTransformIfNotExistAsync(string transformName, AzureMediaServicesClient client)
        {
            string resourceGroupName = _configuration["AzureMediaServices:ResourceGroupName"];
            string accountName = _configuration["AzureMediaServices:AccountName"];

            Transform transform = await client.Transforms.GetAsync(resourceGroupName, accountName, transformName);

            if (transform == null)
            {
                TransformOutput[] outputs = new TransformOutput[]
                {
                    new TransformOutput(new BuiltInStandardEncoderPreset(EncoderNamedPreset.AdaptiveStreaming)),
                };

                await client.Transforms.CreateOrUpdateAsync(resourceGroupName, accountName, transformName, outputs);

                #region cutom transform
                //TransformOutput[] outputs = new TransformOutput[]
                //{

                //new TransformOutput(
                //      new StandardEncoderPreset(
                //          codecs: new Codec[]
                //          {
                //        // Add an AAC Audio layer for the audio encoding
                //        new AacAudio(
                //            channels: 2,
                //            samplingRate: 48000,
                //            bitrate: 128000,
                //            profile: AacAudioProfile.AacLc
                //        ),
                //        // Next, add a H264Video for the video encoding
                //       new H264Video (
                //             // Add H264Layers. Assign a label that you can use for the output filename
                //            layers:  new H264Layer[]
                //            {
                //                new H264Layer (
                //                    bitrate: 3600000,
                //                    width: "1280",
                //                    height: "720",
                //                    label: "HD-3600kbps"
                //                ),
                //                new H264Layer (
                //                    bitrate: 1600000,
                //                    width: "854",
                //                    height: "480",
                //                    label: "SD-1600kbps"
                //                ),
                //                new H264Layer (
                //                    bitrate: 600000,
                //                    width: "640",
                //                    height: "360",
                //                    label: "SD-600kbps"
                //                ),
                //                new H264Layer (
                //                    bitrate: 300000,
                //                    width: "426",
                //                    height: "240",
                //                    label: "SD-300kbps"
                //                ),
                //            }
                //        )},

                //          // Specify the format for the output files - for video+audio
                //          formats: new Format[]
                //          {
                //              new Mp4Format(filenamePattern:"Video-{Basename}-{Label}-{Bitrate}{Extension}")
                //          }
                //      )
                //  )};

                //string description = "Custom encoding transform with 4 MP4 bitrates";


                #endregion
            }
        }

        public async Task<MediaServicesEncodingJobStatus> DeleteFinishedEncodingJobAsync(string videoCode)
        {
            string jobName = $"job-{videoCode}";
            string locatorName = $"locator-{videoCode}";
            string outputAssetName = $"output-{videoCode}";
            string transformName = _configuration["AzureMediaServices:TransformName"];
            string resourceGroupName = _configuration["AzureMediaServices:ResourceGroupName"];
            string accountName = _configuration["AzureMediaServices:AccountName"];
            string policyName = _configuration["AzureMediaServices:ContentKeyPolicyName"];

            AzureMediaServicesClient client = await GetAzureMediaServicesClientAsync();

            var job = await client.Jobs.GetAsync(resourceGroupName, accountName, transformName, jobName);

            var encodingJobStatus = (MediaServicesEncodingJobStatus)Enum.Parse(typeof(MediaServicesEncodingJobStatus), job.State.ToString());

            if (job.State == JobState.Finished)
            {
                if (_configuration["AzureMediaServices:ContentKeyPolicyEnabled"] == true.ToString())
                    await CreateContentKeyPolicyAsync(client, resourceGroupName, accountName, policyName);

                await CreateLocatorAsync(resourceGroupName, accountName, policyName, client, locatorName, outputAssetName);

                await client.Jobs.DeleteAsync(resourceGroupName, accountName, transformName, jobName);
            }

            return encodingJobStatus;
        }
        public async Task<VideoStreamingDto> GetVideoStreamingDtoAsync(string videoCode)
        {
            const string DEFAULT_STREAMING_ENDPOINT_NAME = "default";
            List<string> streamingUrls = new List<string>();
            string locatorName = $"locator-{videoCode}";

            var tokenSigningKey = Encoding.ASCII.GetBytes(_configuration["AzureMediaServices:ContentTokenSigningKey"]);
            string resourceGroupName = _configuration["AzureMediaServices:ResourceGroupName"];
            string accountName = _configuration["AzureMediaServices:AccountName"];
            string contentKeyIssuer = _configuration["AzureMediaServices:ContentKeyIssuer"];
            string contentKeyAudience = _configuration["AzureMediaServices:ContentKeyAudience"];

            try
            {
                var client = await GetAzureMediaServicesClientAsync();
                StreamingEndpoint streamingEndpoint = await client.StreamingEndpoints.GetAsync(resourceGroupName, accountName, DEFAULT_STREAMING_ENDPOINT_NAME);

                if (streamingEndpoint != null)
                {
                    if (streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
                    {
                        await client.StreamingEndpoints.StartAsync(resourceGroupName, resourceGroupName, DEFAULT_STREAMING_ENDPOINT_NAME);
                    }
                }

                ListPathsResponse paths = await client.StreamingLocators.ListPathsAsync(resourceGroupName, accountName, locatorName);
                string path = paths.StreamingPaths.FirstOrDefault(p => p.StreamingProtocol == StreamingPolicyStreamingProtocol.SmoothStreaming).Paths.FirstOrDefault();

                string token = null;
                if (_configuration["AzureMediaServices:ContentKeyPolicyEnabled"] == true.ToString())
                {
                    var locator = await client.StreamingLocators.GetAsync(resourceGroupName, accountName, locatorName);

                    if (locator.ContentKeys.Any())
                    {
                        var contentKeyIdentitfier = locator.ContentKeys.FirstOrDefault().Id.ToString();
                        token = GetStreamingToken(contentKeyIssuer, contentKeyAudience, contentKeyIdentitfier, tokenSigningKey);
                    }
                }

                UriBuilder uriBuilder = new UriBuilder
                {
                    Scheme = "https",
                    Host = streamingEndpoint.HostName,
                    Path = path
                };

                return new VideoStreamingDto
                {
                    Url = uriBuilder.ToString(),
                    AesToken = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while getting streaming URL: {ex.Message}, inner ex: {ex.InnerException}, stack trace: {ex.StackTrace}");
                return null;
            }
        }
        public string GetStreamingToken(string issuer, string audience, string keyIdentifier, byte[] tokenVerificationKey)
        {
            var tokenSigningKey = new SymmetricSecurityKey(tokenVerificationKey);

            SigningCredentials cred = new SigningCredentials(
                tokenSigningKey,
                // Use the  HmacSha256 and not the HmacSha256Signature option, or the token will not work!
                SecurityAlgorithms.HmacSha256,
                SecurityAlgorithms.Sha256Digest);

            Claim[] claims = new Claim[]
            {
                new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier)
            };

            var expiryMinutes = _configuration["AzureMediaServices:ContentTokenExpiryInMinutes"];
            if (string.IsNullOrEmpty(expiryMinutes)) expiryMinutes = "60";

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expiryMinutes)),
                signingCredentials: cred);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }
        public async Task CreateContentKeyPolicyAsync(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string contentKeyPolicyName)
        {
            ContentKeyPolicy policy = await client.ContentKeyPolicies.GetAsync(resourceGroupName, accountName, contentKeyPolicyName);

            if (policy == null)
            {
                var tokenSigningKey = Encoding.ASCII.GetBytes(_configuration["AzureMediaServices:ContentTokenSigningKey"]);
                ContentKeyPolicySymmetricTokenKey primaryKey = new ContentKeyPolicySymmetricTokenKey(tokenSigningKey);
                List<ContentKeyPolicyTokenClaim> requiredClaims = new List<ContentKeyPolicyTokenClaim>()
                {
                    ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim
                };

                List<ContentKeyPolicyOption> options = new List<ContentKeyPolicyOption>()
                {
                    new ContentKeyPolicyOption(
                        new ContentKeyPolicyClearKeyConfiguration(),
                        new ContentKeyPolicyTokenRestriction(_configuration["AzureMediaServices:ContentKeyIssuer"],_configuration["AzureMediaServices:ContentKeyAudience"], primaryKey,
                            ContentKeyPolicyRestrictionTokenType.Jwt, null, requiredClaims))
                };

                await client.ContentKeyPolicies.CreateOrUpdateAsync(resourceGroupName, accountName, contentKeyPolicyName, options);
            }
        }

        public async Task CreateLocatorAsync(string resourceGroupName, string accountName, string contentKeyPolicyName, AzureMediaServicesClient client, string locatorName, string outputAssetName)
        {
            if (_configuration["AzureMediaServices:ContentKeyPolicyEnabled"] == true.ToString())
                await client.StreamingLocators.CreateAsync(resourceGroupName, accountName, locatorName, new StreamingLocator { AssetName = outputAssetName, StreamingPolicyName = PredefinedStreamingPolicy.ClearKey, DefaultContentKeyPolicyName = contentKeyPolicyName });
            else
                await client.StreamingLocators.CreateAsync(resourceGroupName, accountName, locatorName, new StreamingLocator { AssetName = outputAssetName, StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly });
        }

        public async Task<AzureMediaServicesClient> GetAzureMediaServicesClientAsync()
        {
            ClientCredential clientCredential = new ClientCredential(_configuration["AzureMediaServices:AadClientId"], _configuration["AzureMediaServices:AadSecret"]);

            Microsoft.Rest.ServiceClientCredentials serviceClientCredential = await ApplicationTokenProvider.LoginSilentAsync(_configuration["AzureMediaServices:AadTenantId"], clientCredential, ActiveDirectoryServiceSettings.Azure);


            var client = new AzureMediaServicesClient(new Uri(_configuration["AzureMediaServices:ArmEndpoint"]), serviceClientCredential)
            {
                SubscriptionId = _configuration["AzureMediaServices:SubscriptionId"]
            };
            return client;
        }
    }
}
