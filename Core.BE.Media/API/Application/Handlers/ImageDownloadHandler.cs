//using System.IO;
//using System.Threading.Tasks;
//using Emeint.Core.BE.Domain.Exceptions;
//using Emeint.Core.BE.Media.Domain.Enums;
//using Emeint.Core.BE.Media.Domain.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace Emeint.Core.BE.Media.API.Application.Handlers
//{
//    public class ImageDownloadHandler
//    {
//        private readonly IImageRepository _imageRepository;

//        public ImageDownloadHandler()
//        {
//        }

//        public ImageDownloadHandler(IImageRepository imageRepository)
//        {
//            _imageRepository = imageRepository;
//        }

//        public async Task<IActionResult> GetImage(string image_path, int? width, int? height, ImageScalingMode? scalingMode)
//        {
//            if (image_path == null) throw new MissingParameterException(nameof(image_path));

//            MemoryStream memoryStream = new MemoryStream();
//            FileStream fileStream = null;
//            ContentTypeHandler contentTypeHandler = new ContentTypeHandler();

//            //extract extension from image_path
//            //var ext = image_path.Split(".")[1];

//            // full path to file 
//            var path = Path.Combine(
//                Directory.GetCurrentDirectory(), "wwwroot", "imgapi",
//                image_path);

//            if (File.Exists(path))
//            {
//                using (fileStream = new FileStream(path, FileMode.Open))
//                {

//                    await fileStream.CopyToAsync(memoryStream);
//                }
//                memoryStream.Position = 0;
//                return new FileContentResult(memoryStream.ToArray(), contentTypeHandler.GetContentType(path));
//            }

//            var image = _imageRepository.GetImageByCode(image_path) ?? throw new InvalidParameterException(nameof(image_path),image_path);
//            memoryStream = new MemoryStream(image.BinaryData);
//            using (fileStream = new FileStream(path, FileMode.Create))
//            {
//                await memoryStream.CopyToAsync(fileStream);
//            }
//            return new FileContentResult(image.BinaryData, contentTypeHandler.GetContentType(path));
//        }
//    }
//}
