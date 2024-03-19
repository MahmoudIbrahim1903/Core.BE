using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Exceptions;
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
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}")]
    public class AudioController : Controller
    {
        private readonly IMediaStorageManager _mediaStorageManager;
        public AudioController(IMediaStorageManager mediaStorageManager)
        {
            _mediaStorageManager = mediaStorageManager;
        }

        /// <summary>
        /// Upload Audio
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<Response<AudioViewModel>> Post(IFormFile audio)
        {
            if (audio == null)
                throw new MissingParameterException("audio");

            var audioVm = await _mediaStorageManager.UploadAudioAsync(audio);

            return new Response<AudioViewModel>
            {
                Data = audioVm
            };
        }

    }
}
