using System.Threading.Tasks;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.API.Application.Handlers;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Media.Domain.Configurations;
using Emeint.Core.BE.Media.Managers;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Media.API.Controllers
{
    [Produces("application/json")]
    //[Route("api/image")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}")]
    public class ImageController : Controller
    {
        private readonly ImageUploadHandler _imageUploadHandler;
        private readonly IIdentityService _identityService;
        public ImageController(ImageUploadHandler imageUploadHandler, IIdentityService identityService)
        {
            _imageUploadHandler = imageUploadHandler;
            _identityService = identityService;
        }

        ///// <summary>
        ///// /imgapi/(image_name)
        ///// </summary>
        ///// <param name="image_path"></param>
        ///// <param name="w"></param>
        ///// <param name="h"></param>
        ///// <param name="autorotate"></param>
        ///// <param name="format"></param>
        ///// <param name="crop">pad, max, crop, stretch</param>
        //// GET: Images/image_path
        //[Route("images")]
        //[HttpGet]
        //public void DownloadImage(string image_path, int? w, int? h, bool? autorotate, string format, string crop)
        //{
        //}

        //// GET: api/Image/5
        //[HttpGet]
        //[Route("download_image")]
        //public IActionResult DownloadImage(string image_path, int? width, int? height, ImageScalingMode? scalingMode)
        //{
        //    if (image_path == null) throw new MissingParameterException(nameof(image_path));
        //    var image = new ImageDownloadHandler(_imageRepository);
        //    var result = image.GetImage(image_path, width, height, scalingMode);

        //    if (!result.IsCompletedSuccessfully)
        //        throw new FailedToLoadFileException(image_path, result.Exception.Message);

        //    return result.Result;
        //}

        // POST: api/Image
        [Route("upload")]
        [Authorize]
        [HttpPost]
        public async Task<Response<ImageViewModel>> UploadImage(IFormFile file, [FromQuery]string client_reference_number)
        {
            if (file == null)
                throw new MissingParameterException("file");
            if (string.IsNullOrEmpty(client_reference_number))
                throw new MissingParameterException("client_reference_number");

            var imageVM = await _imageUploadHandler.SaveImage(file, client_reference_number, _identityService.TenantCode, _identityService.DisplayName);

            return new Response<ImageViewModel> { Data = imageVM };
        }

        // PUT: api/Image/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[Route("delete_image")]
        //[Authorize]
        //[HttpDelete]
        //public void DeleteImage(string image_code)
        //{
        //    if (image_code == null)
        //        throw new MissingParameterException(nameof(image_code));

        //    var image = _imageRepository.GetImageByCode(image_code);
        //    if (image == null)
        //        throw new InvalidParameterException(nameof(image_code), image_code);

        //    _imageRepository.DeleteByCode(image_code);

        //    if (Directory.Exists(ImagePathUtility.GetImageFolderPath(image_code)))
        //    {
        //        Directory.Delete(ImagePathUtility.GetImageFolderPath(image_code), true);
        //    }

        //    _imageRepository.SaveEntities();
        //}
    }
}
