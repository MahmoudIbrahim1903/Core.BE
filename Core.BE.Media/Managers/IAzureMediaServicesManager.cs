using Emeint.Core.BE.Media.API.Application.ValueObject.Dtos;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Enums;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public interface IAzureMediaServicesManager
    {
        Task<MediaServicesEncodingJobStatus> StartVideoEncodingJobAsync(string videoUrl, string videoCode);
        Task<Job> CreateEncodingJobAsync(string videoUrl, string videoCode, string transformName, string jobName, string outputAssetName, AzureMediaServicesClient client);
        Task CreateTransformIfNotExistAsync(string transformName, AzureMediaServicesClient client);
        Task<MediaServicesEncodingJobStatus> DeleteFinishedEncodingJobAsync(string videoCode);
        Task<VideoStreamingDto> GetVideoStreamingDtoAsync(string videoCode);
        string GetStreamingToken(string issuer, string audience, string keyIdentifier, byte[] tokenVerificationKey);
        Task CreateContentKeyPolicyAsync(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string contentKeyPolicyName);
        Task CreateLocatorAsync(string resourceGroupName, string accountName, string contentKeyPolicyName, AzureMediaServicesClient client, string locatorName, string outputAssetName);
        Task<AzureMediaServicesClient> GetAzureMediaServicesClientAsync();
    }
}
