//using SkiaSharp;
using System.IO;
using Emeint.Core.BE.Media.API.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Emeint.Core.BE.Media.API.Application.Handlers;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System.Linq;

namespace Emeint.Core.BE.Media.Utilities
{
    public static class ImagePathUtility
    {

        public static string GetImageFolderPath(string imagePath, bool combineLocalStoragePath = true)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new MissingParameterException("imagePath");

            //Creating file path, spilting on '_' then select index[1] then replace all the '-' with '\\' 
            var folderStructure = imagePath.Split("_")[1].Replace('-', Path.DirectorySeparatorChar);

            //Split extension from imagePath and add '\\' to the beginning of the string and concat it to the folder structure
            var image = imagePath.Split(".")[0].TrimStart('/').Split('/');
            folderStructure += $"{Path.DirectorySeparatorChar}{image[image.Length - 1]}";

            if (!combineLocalStoragePath)
                return Path.Combine("imgapi", folderStructure);
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgapi", folderStructure);
        }

        public static string GetImageFilePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new MissingParameterException("imagePath");

            var folderStructure = GetImageFolderPath(imagePath);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgapi", folderStructure, imagePath);

            return path;
        }

        public static string GetImageQueryPath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new MissingParameterException("imagePath");

            //Creating file path, spilting on '_' then select index[1] then replace all the '-' with '\\' 
            var folderStructure = $"/imgapi/{imagePath.Split("_")[1].Replace('-', '/')}";

            //Split extension from imagePath and add '\\' to the beginning of the string and concat it to the folder structure
            var image = imagePath.Split(".")[0].TrimStart('/').Split('/');
            folderStructure += $"/{image[image.Length - 1]}";
            image = imagePath.Split('/');
            folderStructure += $"/{image[image.Length - 1]}";
            var path = folderStructure;

            return path;
        }

        public static string GetImageNameWithNewWidthHeight(string imagePath, int w, int h)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new MissingParameterException("imagePath");

            var originalPath = GetImageFilePath(imagePath);
            var filePathList = originalPath.Split("\\");
            var splittedFilePath = filePathList[filePathList.Length - 1].Split(".");
            // splittedFilePath[0] -> fileName
            // splittedFilePath[1] -> ext

            var appendedFilePath = $"{originalPath.Split(filePathList[filePathList.Length - 1])[0]}{splittedFilePath[0]}_{w}x{h}";

            if (splittedFilePath.Length > 1)
                appendedFilePath = appendedFilePath + "." + splittedFilePath[1];

            return appendedFilePath;
        }

        //public static void SaveImageOnDisk(string imagePath, SKData imageData, int w, int h)
        //{
        //    if (string.IsNullOrEmpty(imagePath))
        //        throw new MissingParameterException("imagePath");

        //    if (imageData == null)
        //        throw new MissingParameterException("imageData");

        //    var originalPath = GetImageFilePath(imagePath);
        //    var filePathList = originalPath.Split("\\");
        //    var splittedFilePath = filePathList[filePathList.Length - 1].Split(".");
        //    // splittedFilePath[0] -> fileName
        //    // splittedFilePath[1] -> ext
        //    var appendedFilePath = $"{originalPath.Split(filePathList[filePathList.Length - 1])[0]}{splittedFilePath[0]}_{w}x{h}.{splittedFilePath[1]}";

        //    //cache the result to local disk
        //    var fileStream = new FileStream(appendedFilePath, FileMode.Create);
        //    var stream = imageData.AsStream();
        //    stream.CopyToAsync(fileStream);

        //    // cleanup
        //    stream.Dispose();
        //    fileStream.Dispose();
        //}

        public static string GetFileExtension(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new MissingParameterException("imagePath");

            return Path.GetExtension(imagePath);
        }

        public static bool IsAbsoluteUrl(string text)
        {
            if (text != null && text.Contains("http"))
            {
                return true;
            }

            return false;
        }
        public static bool IsRelativeUrl(string text)
        {
            if (text != null && text.StartsWith("/"))
            {
                return true;
            }

            return false;
        }

        public static string GetImageFullyPath(string imageCode)
        {
            if (string.IsNullOrEmpty(imageCode))
                return string.Empty;

            var imageWithoutExtension = imageCode.Split(".")[0];

            var imageName = imageCode.Split("_")[1];
            var imageUrl = imageName.Split("-");

            var yearFolder = imageUrl[0];
            var monthFolder = imageUrl[1];
            var dayFolder = imageUrl[2];
            var hourFolder = imageUrl[3];

            string webRootPath = Directory.GetCurrentDirectory() + "/wwwroot";

            string imageBasePath = webRootPath + "/imgapi";

            string fullyImagePath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}\{6}", imageBasePath, yearFolder, monthFolder, dayFolder, hourFolder, imageWithoutExtension, imageCode);
            return fullyImagePath;
        }

        public static string GetImgContainer(IConfiguration configs, ResolutionContext context)
        {
            var countryCode = (string)context.Options.Items["CountryCode"];
            var container = configs.GetSection("AzureImageContainerNames")?.GetChildren().FirstOrDefault(ch => ch.Key == countryCode)?.Value;

            if (container == null)
            {
                return configs.GetSection("AzureImageContainerNames")?.GetChildren().FirstOrDefault().Value;
            }
            return container;
        }

        public static string GetImgContainer(IConfiguration configs, string countryCode)
        {
            var container = configs.GetSection("AzureImageContainerNames")?.GetChildren().FirstOrDefault(ch => ch.Key == countryCode)?.Value;

            if (container == null)
            {
                return configs.GetSection("AzureImageContainerNames")?.GetChildren().FirstOrDefault().Value;
            }
            return container;
        }

        //public static SKData GetCachedImageFromLocal(string imagePath)
        //{
        //    SKData imageData;
        //    FileStream fileStream = null;
        //    ContentTypeHandler contentTypeHandler = new ContentTypeHandler();

        //    using ( fileStream = new FileStream(imagePath, FileMode.Open))
        //    {

        //         fileStream.CopyToAsync(imageData);
        //    }

        //    return new FileContentResult(memoryStream.ToArray(), contentTypeHandler.GetContentType(imagePath));
        //}
    }

}