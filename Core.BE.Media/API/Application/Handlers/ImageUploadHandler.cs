using System;
using System.IO;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Configurations;
using Emeint.Core.BE.Media.Domain.Enums;
using Emeint.Core.BE.Media.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Managers;
using Emeint.Core.BE.Media.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emeint.Core.BE.Media.API.Application.Handlers
{
    public class ImageUploadHandler
    {
        private readonly IImageRepository _imageRepository;
        private readonly IConfigurationManager _configurationManager;
        private readonly IMediaStorageManager _mediaStorageManager;
        private readonly IConfiguration _configuration;
        private readonly IIdentityService _identityService;
        public ImageUploadHandler()
        {
        }

        public ImageUploadHandler(IImageRepository imageRepository,
             IConfigurationManager configurationManager,
             IMediaStorageManager mediaStorageManager,
             IConfiguration configuration,
             IIdentityService identityService)
        {
            _imageRepository = imageRepository;
            _configurationManager = configurationManager;
            _mediaStorageManager = mediaStorageManager;
            _configuration = configuration;
            _identityService = identityService;
        }

        public async Task<ImageViewModel> SaveImage(IFormFile image, string clientReferenceNumber, string tenantCode, string createdBy)
        {
            //Null checks
            if (string.IsNullOrEmpty(clientReferenceNumber))
                throw new MissingParameterException("clientReferenceNumber");
            if (image == null)
                throw new MissingParameterException("image");

            if (_imageRepository.ImageClientReferenceNumberExists(clientReferenceNumber))
                throw new ImageAlreadyExistException(clientReferenceNumber);

            if (image.Length / 1000 > _configurationManager.GetImageMaxSize())
                throw new MaxImageSizeExceededException(_configurationManager.GetImageMaxSize().ToString());

            //Mapping from ViewModel to Entity
            var imgMetaData = new ImageMetaData(image, clientReferenceNumber, tenantCode, createdBy);

            var imgPath = Path.Combine(ImagePathUtility.GetImageFolderPath(imgMetaData.Code, false), imgMetaData.Code);

            await _mediaStorageManager.UploadImageAsync(imgMetaData.ImageData.BinaryData, imgPath, imgMetaData);


            var azureStorageBasePath = _configuration["AzureStorageBasePath"];
            bool buildAzureStorageFullUrl = bool.Parse(_configuration["BuildAzureStorageFullUrl"]);
            var container = ImagePathUtility.GetImgContainer(_configuration, _identityService.CountryCode);

            //Returning Image ViewModel
            return new ImageViewModel(imgMetaData.Code, _configurationManager.GetImagesRootFolder(), buildAzureStorageFullUrl, azureStorageBasePath, container);
        }
    }
}
