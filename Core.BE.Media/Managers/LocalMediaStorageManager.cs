using Emeint.Core.BE.Media.API.Application.ValueObject.Dtos;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Configurations;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public class LocalMediaStorageManager : IMediaStorageManager
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IImageRepository _imageRepository;
        private readonly IHostingEnvironment _env;
        public LocalMediaStorageManager(IConfigurationManager configurationManager, IImageRepository imageRepository, IHostingEnvironment env)
        {
            _configurationManager = configurationManager;
            _env = env;
            _imageRepository = imageRepository;
        }
        public async Task<System.Drawing.Image> ReadImageAsync(string path)
        {
            var imagePath = Path.Combine(
                _env.WebRootPath,
                path.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

            var exists = await ImageExistsAsync(imagePath);
            if (exists)
                return System.Drawing.Image.FromFile(imagePath);
            else
                return null;
        }

        public async Task<bool> ImageExistsAsync(string path)
        {
            bool exists = false;
            // get the image location on disk
            var imagePath = Path.Combine(
                _env.WebRootPath,
                path.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

            if (!File.Exists(imagePath))
            {
                exists = false;
                var imageCode = imagePath.Split("\\").Last();
                var image = _imageRepository.GetImageByCode(imageCode);
                if (image != null && image.ImageData != null && image.ImageData.BinaryData != null)
                {
                    Directory.CreateDirectory(ImagePathUtility.GetImageFolderPath(imageCode));
                    File.WriteAllBytes(imagePath, image.ImageData.BinaryData);
                    exists = true;
                }

                #region Removed skia sharp
                //var skData = SKData.CreateCopy(image.BinaryData);
                //using (var fileStream = new FileStream(imagePath, FileMode.Create))
                //{
                //    skData.SaveTo(fileStream);
                //}
                #endregion
            }
            else
            {
                exists = true;
            }
            return exists;
        }

        public async Task WriteToResponse(RequestDelegate requestDelegate, HttpContext context, string path)
        {
            context.Request.Path = path;
            await requestDelegate.Invoke(context);
        }

        public async Task<string> UploadImageAsync(byte[] binaryData, string imagePath, ImageMetaData imageMetaData = null)
        {
            using (var memoryStream = new MemoryStream(binaryData))
            {

                imagePath = Path.Combine(_env.WebRootPath,
                       imagePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

                //Ensure folders are created
                Directory.CreateDirectory(imagePath.Remove(imagePath.LastIndexOf('\\')));

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    //save to local file
                    memoryStream.CopyToAsync(fileStream).Wait();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
            }

            //saving image metadata to database
            if (imageMetaData != null)
            {
                //clear image data
                if (!_configurationManager.SaveImageBinaryDataToDataBase())
                    imageMetaData.ImageData = null;

                //Saving into database 
                await _imageRepository.AddAsync(imageMetaData);
                await _imageRepository.SaveEntitiesAsync();
            }

            return imagePath;
        }

        public async Task DeleteImageAsync(string imageCode)
        {
            //delete image from db
            var image = _imageRepository.GetImageByCode(imageCode);
            if (image != null)
            {
                await _imageRepository.DeleteByCode(imageCode);
                await _imageRepository.SaveEntitiesAsync();
            }

            //delete image from disk
            var rootFloder = _configurationManager.GetImagesRootFolder();
            var path = rootFloder + imageCode;
            path = ImagePathUtility.GetImageQueryPath(path);
            path = Path.Combine(
            _env.WebRootPath,
            path.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

            var directoryPath = path.Substring(0, path.LastIndexOf('\\'));
            Directory.Delete(directoryPath, true);
        }

        public Task<string> DownloadVideoAsync(string videoCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<VideoViewModel> UploadVideoAsync(IFormFile formFile)
        {
            throw new System.NotImplementedException();
        }

        Task<VideoDto> IMediaStorageManager.GetVideoAsync(string videoCode)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteVideoAsync(string videoCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<DocumentViewModel> UploadDocumentAsync(IFormFile document)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> UploadExcelDocumentAsync(FileInfo file, string contentType)
        {
            throw new System.NotImplementedException();
        }

        public Task<DocumentViewModel> GetDocumentAsync(string documentCode)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteDocumentAsync(string documentCode, string deletedBy)
        {
            throw new System.NotImplementedException();
        }

        public string GetVideoNameByCode(string videoCode)
        {
            throw new System.NotImplementedException();
        }

        public string GetDocumentNameByCode(string documentCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetVideoUrlAsync(string videoPath, string containerName, string accountName, string accountKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<AudioViewModel> UploadAudioAsync(IFormFile formFile)
        {
            throw new System.NotImplementedException();
        }

        public Task<AudioViewModel> GetAudioAsync(string audioCode)
        {
            throw new System.NotImplementedException();
        }
    }
}
