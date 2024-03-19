using System;
using System.IO;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Utilities;
using Microsoft.AspNetCore.Http;

namespace Emeint.Core.BE.Media.API.Application.Handlers
{
    public class ImageDeleteHandler
    {
        private readonly IImageRepository _imageRepository;

        public ImageDeleteHandler()
        {
        }

        public ImageDeleteHandler(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task DeleteImage(string imageCode)
        {

            if (imageCode == null)
                throw new MissingParameterException(nameof(imageCode));

            bool imageExists = _imageRepository.ImageCodeExists(imageCode);
            if (!imageExists)
                throw new InvalidParameterException(nameof(imageCode), imageCode);

            await _imageRepository.DeleteByCode(imageCode);

            if (Directory.Exists(ImagePathUtility.GetImageFolderPath(imageCode)))
            {
                try
                {
                    Directory.Delete(ImagePathUtility.GetImageFolderPath(imageCode), true);
                }
                catch (IOException ex)
                {
                    throw new DeleteFileFailedException(imageCode, ex.Message);
                }
                catch (Exception ex)
                {
                    throw new DeleteFileFailedException(imageCode, ex.Message);
                }
            }

           await _imageRepository.SaveEntitiesAsync();
        }
    }
}