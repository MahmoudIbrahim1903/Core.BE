using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.API.Application.ValueObject.Dtos;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.API.Controllers
{
    [Produces("application/json")]
    //[Route("api/video")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}")]
    public class VideoController : Controller
    {
        private readonly IMediaStorageManager _mediaStorageManager;
        public VideoController(IMediaStorageManager mediaStorageManager)
        {
            _mediaStorageManager = mediaStorageManager;
        }

        /// <summary>
        /// Upload video
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<Response<VideoViewModel>> Post(IFormFile video)
        {
            if (video == null)
                throw new MissingParameterException("video");

            var videoVm = await _mediaStorageManager.UploadVideoAsync(video);

            return new Response<VideoViewModel>
            {
                Data = videoVm
            };
        }

    }
}
