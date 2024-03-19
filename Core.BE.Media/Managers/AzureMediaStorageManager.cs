using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.API.Application.ValueObject.Dtos;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Configurations;
using Emeint.Core.BE.Media.Domain.Enums;
using Emeint.Core.BE.Media.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public class AzureMediaStorageManager : IMediaStorageManager
    {
        ILogger<AzureMediaStorageManager> _logger;
        private readonly IImageRepository _imageRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IIdentityService _identityService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;
        private readonly IAzureMediaServicesManager _azureMediaServicesManager;
        private readonly IAudioRepository _audioRepository;
        private string _imagesContainerName;
        private string _filesContainerName;
        public AzureMediaStorageManager(ILogger<AzureMediaStorageManager> logger, IImageRepository imageRepository, IVideoRepository videoRepository, IIdentityService identityService, IDocumentRepository documentRepository, IConfiguration configuration, IAzureMediaServicesManager azureMediaServicesManager, IAudioRepository audioRepository)
        {
            _logger = logger;
            _imageRepository = imageRepository;
            _videoRepository = videoRepository;
            _identityService = identityService;
            _documentRepository = documentRepository;
            _configuration = configuration; //read images configurations from appsettings , ConfigurationsDbContext can not accessed correctly from ImageDownloadMiddleware
            _azureMediaServicesManager = azureMediaServicesManager;
            _audioRepository = audioRepository;
            _imagesContainerName = GetImagesContainerName();
            _filesContainerName = GetFilesContainerName();
        }

        #region Blobs
        private CloudBlockBlob GetBlockBlobReference(string fileName, string containerName)
        {
            return GetBlockBlobReference(fileName, containerName, _configuration["AzureAccountName"], _configuration["AzureAccountKey"]);
        }

        private CloudBlockBlob GetBlockBlobReference(string fileName, string containerName, string accountName, string accountKey)
        {
            CloudBlobContainer container = GetCloudBlobContainer(containerName, accountName, accountKey);

            return container.GetBlockBlobReference(fileName);
        }

        private CloudBlobContainer GetCloudBlobContainer(string containerName)
        {
            return GetCloudBlobContainer(containerName, _configuration["AzureAccountName"], _configuration["AzureAccountKey"]);
        }

        private CloudBlobContainer GetCloudBlobContainer(string containerName, string accountName, string accountKey)
        {
            StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);

            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            return container;
        }

        private async Task DeleteBlobsInDirectoryAsync(string directoryPath, string containerName)
        {
            var container = GetCloudBlobContainer(containerName);
            var blobDirectory = container.GetDirectoryReference(directoryPath);

            var blobs = blobDirectory.ListBlobs().ToList();
            foreach (IListBlobItem item in blobs)
            {
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(((CloudBlockBlob)item).Name);
                await cloudBlockBlob.DeleteAsync();
            }
        }
        #endregion

        #region Images
        public async Task<System.Drawing.Image> ReadImageAsync(string azureFilePath)
        {
            azureFilePath = azureFilePath.TrimStart('/');
            var blockBlob = GetBlockBlobReference(azureFilePath, _imagesContainerName);
            using (var imageStream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(imageStream);
                imageStream.Seek(0, SeekOrigin.Begin);
                return System.Drawing.Image.FromStream(imageStream);
            }
        }

        public async Task<bool> ImageExistsAsync(string azureFilePath)
        {
            azureFilePath = azureFilePath.TrimStart('/');
            var blockBlob = GetBlockBlobReference(azureFilePath, _imagesContainerName);
            return await blockBlob.ExistsAsync();
        }

        public async Task<string> UploadImageAsync(byte[] binaryData, string imagePath, ImageMetaData imageMetaData = null)
        {
            using (var memoryStream = new MemoryStream(binaryData))
            {
                imagePath = imagePath.TrimStart('/');
                var blockBlob = GetBlockBlobReference(imagePath, _imagesContainerName);
                blockBlob.Properties.ContentType = Path.GetExtension(imagePath) == "png" ? "image/png" : "image/jpeg";

                await blockBlob.UploadFromStreamAsync(memoryStream);
                var blobUri = blockBlob.Uri.ToString();

                //saving image metadata to database
                if (imageMetaData != null)
                {
                    //clear image data
                    imageMetaData.ImageData = null;

                    //Saving into database 
                    await _imageRepository.AddAsync(imageMetaData);

                }

                return imagePath;
            }
        }

        public async Task WriteToResponse(RequestDelegate requestDelegate, HttpContext context, string path)
        {
            //redirect to azure
            context.Response.Redirect($"{_configuration["AzureStorageBasePath"]}{_imagesContainerName}{path}");
        }

        public async Task DeleteImageAsync(string imageCode)
        {
            //delete image from db

            await _imageRepository.DeleteByCode(imageCode);
            await _imageRepository.SaveEntitiesAsync();

            //delete image from azure
            var rootFloder = _configuration["ImagesRootFolder"];
            var path = rootFloder + imageCode;
            path = ImagePathUtility.GetImageQueryPath(path);
            path = path.TrimStart('/');

            var directoryPath = path.Substring(0, path.LastIndexOf('/'));
            await DeleteBlobsInDirectoryAsync(directoryPath, _imagesContainerName);
        }
        #endregion

        #region Videos
        public async Task<VideoViewModel> UploadVideoAsync(IFormFile formFile)
        {
            var videoMaxSizeString = _configuration["VideosMaxSizeInMegaBytes"];
            if (string.IsNullOrEmpty(videoMaxSizeString))
                videoMaxSizeString = "500";
            var videoMaxSize = Convert.ToInt32(videoMaxSizeString);

            if (formFile.Length > videoMaxSize * 1024 * 1024)
                throw new MaxVideoSizeExceededException(videoMaxSize.ToString());

            var ext = Path.GetExtension(formFile.FileName);
            var code = $"VID_{ DateTime.UtcNow.ToString("yyyy-MM-dd-HH-ss")}_{ new Random().Next(1, 100000)}{ext}";
            var fileName = $"videoapi/{code}";

            var blob = GetBlockBlobReference(fileName, _configuration["AzureVideoContainer"]);
            blob.Properties.ContentType = formFile.ContentType;

            await blob.UploadFromStreamAsync(formFile.OpenReadStream());
            bool mediaServicesEnabled = bool.Parse(_configuration["AzureMediaServices:Enabled"]);

            string azureStorageSharedAccessKeyExpiryInMinutes = _configuration["AzureStorageSharedAccessKeyExpiryInMinutes"];
            if (string.IsNullOrEmpty(azureStorageSharedAccessKeyExpiryInMinutes))
                azureStorageSharedAccessKeyExpiryInMinutes = "120";

            var sharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(azureStorageSharedAccessKeyExpiryInMinutes))
            });

            var videoUrl = string.Format("{0}{1}", blob.Uri, sharedAccessSignature);

            var video = await _videoRepository.AddAsync(new Domain.Models.Video
            {
                FileName = formFile.FileName,
                Code = code,
                CreatedBy = _identityService.DisplayName,
                CreationDate = DateTime.UtcNow,
                Url = blob.Uri.ToString()
            });

            var videoVm = new VideoViewModel
            {
                Url = videoUrl,
                Code = code,
                Name = formFile.FileName
            };

            if (mediaServicesEnabled)
            {
                var encodingJobStatus = await _azureMediaServicesManager.StartVideoEncodingJobAsync(videoVm.Url, video.Code);

                video.EncodingJobStatus = encodingJobStatus;
                await _videoRepository.UpdateAsync(video);
            }

            return videoVm;
        }
        public async Task<VideoDto> GetVideoAsync(string videoCode)
        {
            var fileName = $"videoapi/{videoCode}";

            Domain.Models.Video video = _videoRepository.GetVideoByCode(videoCode);
            if (video == null)
                throw new InvalidParameterException("video_code", videoCode);

            if (video.EncodingJobStatus.HasValue && video.EncodingJobStatus != MediaServicesEncodingJobStatus.Finished)
            {
                var encodingJobStatus = await _azureMediaServicesManager.DeleteFinishedEncodingJobAsync(video.Code);

                video.EncodingJobStatus = encodingJobStatus;
                await _videoRepository.UpdateAsync(video);


                //Remove original video if needed
                //var fileName = $"videoapi/{video.Code}";
                //var blob = GetBlockBlobReference(fileName, _configurationManager.GetAzureVideoContainer());
                //await blob.DeleteAsync();
            }

            var videoUrl = await GetVideoUrlAsync(fileName, _configuration["AzureVideoContainer"], _configuration["AzureAccountName"], _configuration["AzureAccountKey"]);

            VideoStreamingDto streamingDto = null;

            if (video.EncodingJobStatus == MediaServicesEncodingJobStatus.Finished)
            {
                if (video.StreamingUrl != null && _configuration["AzureMediaServices:ContentKeyPolicyEnabled"] == false.ToString())
                {
                    streamingDto = new VideoStreamingDto
                    {
                        Url = video.StreamingUrl
                    };
                }
                else
                {
                    streamingDto = await _azureMediaServicesManager.GetVideoStreamingDtoAsync(video.Code);
                    if (streamingDto != null && !string.IsNullOrEmpty(streamingDto.Url))
                    {
                        video.StreamingUrl = streamingDto.Url;
                        await _videoRepository.UpdateAsync(video);
                    }
                }
            }
            return new VideoDto
            {
                Url = videoUrl,
                Code = videoCode,
                Name = video.FileName,
                Streaming = streamingDto
            };
        }
        public async Task<string> GetVideoUrlAsync(string videoPath, string containerName, string accountName, string accountKey)
        {

            var blob = GetBlockBlobReference(videoPath, containerName, accountName, accountKey);

            string azureStorageSharedAccessKeyExpiryInMinutes = _configuration["AzureStorageSharedAccessKeyExpiryInMinutes"];
            if (string.IsNullOrEmpty(azureStorageSharedAccessKeyExpiryInMinutes))
                azureStorageSharedAccessKeyExpiryInMinutes = "120";

            var sharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(azureStorageSharedAccessKeyExpiryInMinutes))
            });

            return string.Format("{0}{1}", blob.Uri, sharedAccessSignature);
        }
        public async Task DeleteVideoAsync(string videoCode)
        {
            var fileName = $"videoapi/{videoCode}";
            var blob = GetBlockBlobReference(fileName, _configuration["AzureVideoContainer"]);
            await blob.DeleteAsync();
            await _videoRepository.DeleteVideoByCode(videoCode);
        }
        public string GetVideoNameByCode(string videoCode)
        {
            var video = _videoRepository.GetVideoByCode(videoCode);

            if (video == null)
                throw new InvalidParameterException("video_code", videoCode);
            return video.FileName;
        }
        #endregion

        #region documents
        public async Task<DocumentViewModel> UploadDocumentAsync(IFormFile formFile)
        {
            var ext = Path.GetExtension(formFile.FileName);
            var code = $"DOC_{ DateTime.UtcNow.ToString("yyyy-MM-dd-HH-ss")}_{ new Random().Next(1, 100000)}{ext}";
            var fileName = $"documentapi/{code}";            
            
            var blob = GetBlockBlobReference(fileName, _filesContainerName);
            blob.Properties.ContentType = formFile.ContentType;

            await blob.UploadFromStreamAsync(formFile.OpenReadStream());

            string azureStorageSharedAccessKeyExpiryInMinutes = _configuration["AzureStorageSharedAccessKeyExpiryInMinutes"];
            if (string.IsNullOrEmpty(azureStorageSharedAccessKeyExpiryInMinutes))
                azureStorageSharedAccessKeyExpiryInMinutes = "120";

            var sharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(azureStorageSharedAccessKeyExpiryInMinutes))
            });

            var documentUrl = string.Format("{0}{1}", blob.Uri, sharedAccessSignature);

            await _documentRepository.AddAsync(new Document
            {
                FileName = formFile.FileName,
                Code = code,
                CreatedBy = _identityService.DisplayName,
                CreationDate = DateTime.UtcNow,
                Url = blob.Uri.ToString()
            });

            return new DocumentViewModel
            {
                Url = documentUrl,
                Code = code,
                Name = formFile.FileName
            };
        }

        public async Task<string> UploadExcelDocumentAsync(FileInfo file, string contentType)
        {
            var fileName = $"excelexportdocument/{file.Name}";
            string containerName = _configuration.GetSection("AzureExportExcelContainerNames")?.GetChildren().FirstOrDefault(ch => ch.Key == _identityService.CountryCode)?.Value;
            if(string.IsNullOrEmpty(containerName))
                containerName = _configuration.GetSection("AzureExportExcelContainerNames")?.GetChildren().FirstOrDefault()?.Value;

            var blob = GetBlockBlobReference(fileName, containerName);

            if (!string.IsNullOrEmpty(contentType))
                blob.Properties.ContentType = contentType;
            else
                blob.Properties.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            await blob.UploadFromFileAsync(file.FullName);

            string azureStorageSharedAccessKeyExpiryInMinutes = _configuration["AzureStorageSharedAccessKeyExpiryInMinutes"];
            if (string.IsNullOrEmpty(azureStorageSharedAccessKeyExpiryInMinutes))
                azureStorageSharedAccessKeyExpiryInMinutes = "120";

            var sharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(azureStorageSharedAccessKeyExpiryInMinutes))
            });

            var documentUrl = $"{blob.Uri}{sharedAccessSignature}";

            return documentUrl;
        }

        public async Task<DocumentViewModel> GetDocumentAsync(string documentCode)
        {
            Document document = _documentRepository.GetDocumentByCode(documentCode);
            if (document == null)
                throw new InvalidParameterException("document_code", documentCode);

            var fileName = $"documentapi/{documentCode}";            

            var blob = GetBlockBlobReference(fileName, _filesContainerName);

            string azureStorageSharedAccessKeyExpiryInMinutes = _configuration["AzureStorageSharedAccessKeyExpiryInMinutes"];
            if (string.IsNullOrEmpty(azureStorageSharedAccessKeyExpiryInMinutes))
                azureStorageSharedAccessKeyExpiryInMinutes = "120";

            var sharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(azureStorageSharedAccessKeyExpiryInMinutes))
            });

            var documentUrl = string.Format("{0}{1}", blob.Uri, sharedAccessSignature);
            var documentName = document.FileName;

            return new DocumentViewModel
            {
                Name = documentName,
                Url = documentUrl,
                Code = documentCode
            };
        }

        public async Task DeleteDocumentAsync(string documentCode, string deletedBy)
        {
            var fileName = $"documentapi/{documentCode}";
            var blob = GetBlockBlobReference(fileName, _configuration["AzureDocumentContainer"]);
            await blob.DeleteAsync();
            await _documentRepository.DeleteDocumentByCode(documentCode, deletedBy);
        }

        public string GetDocumentNameByCode(string documentCode)
        {
            var document = _documentRepository.GetDocumentByCode(documentCode);

            if (document == null)
                throw new InvalidParameterException("document_code", documentCode);
            return document.FileName;
        }

        private string GetFilesContainerName()
        {
            string containerName = _configuration.GetSection("AzureUploadContainerNames")?.GetChildren().FirstOrDefault(ch => ch.Key == _identityService.CountryCode)?.Value;
            if (string.IsNullOrEmpty(containerName))
                containerName = _configuration.GetSection("AzureUploadContainerNames")?.GetChildren().FirstOrDefault()?.Value;

            return containerName;
        }

        private string GetImagesContainerName()
        {
            string containerName = _configuration.GetSection("AzureImageContainerNames")?.GetChildren().FirstOrDefault(ch => ch.Key == _identityService.CountryCode)?.Value;
            if (string.IsNullOrEmpty(containerName))
                containerName = _configuration.GetSection("AzureImageContainerNames")?.GetChildren().FirstOrDefault()?.Value;

            return containerName;
        }
        #endregion

        #region Audio
        public async Task<AudioViewModel> UploadAudioAsync(IFormFile formFile)
        {
            var audioMaxSizeString = _configuration["MaxAudioSizeInMegaBytes"];
            if (string.IsNullOrEmpty(audioMaxSizeString))
                audioMaxSizeString = "10";
            var audioMaxSize = Convert.ToInt32(audioMaxSizeString);

            if (formFile.Length > audioMaxSize * 1024 * 1024)
                throw new MaxAudioSizeExceededException(audioMaxSize.ToString());

            var audioAllowedExtensions = _configuration["AudioAllowedExtensions"];
            if (!string.IsNullOrEmpty(audioAllowedExtensions))
            {
                var fileExtension = Path.GetExtension(formFile.FileName);
                if (!audioAllowedExtensions.Split(',').Any(i => i == fileExtension))
                    throw new UnsupportedAudioFileException(fileExtension);
            }

            var ext = Path.GetExtension(formFile.FileName);
            var code = $"AUD_{ DateTime.UtcNow.ToString("yyyy-MM-dd-HH-ss")}_{ new Random().Next(1, 100000)}{ext}";
            var fileName = $"audioapi/{code}";

            var blob = GetBlockBlobReference(fileName, _configuration["AzureAudioContainer"]);
            blob.Properties.ContentType = formFile.ContentType;

            await blob.UploadFromStreamAsync(formFile.OpenReadStream());

            string azureStorageSharedAccessKeyExpiryInMinutes = _configuration["AzureStorageSharedAccessKeyExpiryInMinutes"];
            if (string.IsNullOrEmpty(azureStorageSharedAccessKeyExpiryInMinutes))
                azureStorageSharedAccessKeyExpiryInMinutes = "120";

            var sharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(azureStorageSharedAccessKeyExpiryInMinutes))
            });

            var audioUrl = string.Format("{0}{1}", blob.Uri, sharedAccessSignature);

            var audio = await _audioRepository.AddAsync(new Domain.Models.Audio
            {
                Code = code,
                CreatedBy = _identityService.DisplayName,
                CreationDate = DateTime.UtcNow,
                Url = blob.Uri.ToString()
            });

            var audioVm = new AudioViewModel
            {
                Url = audioUrl,
                Code = code,
            };
            return audioVm;
        }

        public async Task<AudioViewModel> GetAudioAsync(string audioCode)
        {
            var fileName = $"audioapi/{audioCode}";

            Domain.Models.Audio audio = _audioRepository.GetAudioByCode(audioCode);
            if (audio == null)
                throw new InvalidParameterException("audio_code", audioCode);

            var blob = GetBlockBlobReference(fileName, _configuration["AzureAudioContainer"]);

            string azureStorageSharedAccessKeyExpiryInMinutes = _configuration["AzureStorageSharedAccessKeyExpiryInMinutes"];
            if (string.IsNullOrEmpty(azureStorageSharedAccessKeyExpiryInMinutes))
                azureStorageSharedAccessKeyExpiryInMinutes = "120";

            var sharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(azureStorageSharedAccessKeyExpiryInMinutes))
            });

            var audioUrl = string.Format("{0}{1}", blob.Uri, sharedAccessSignature);

            return new AudioViewModel
            {
                Url = audioUrl,
                Code = audioCode,
            };
        }
        #endregion
    }
}