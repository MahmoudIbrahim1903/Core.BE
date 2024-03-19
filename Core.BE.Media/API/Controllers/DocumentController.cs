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
    public class DocumentController : Controller
    {
        private readonly IMediaStorageManager _mediaStorageManager;
        public DocumentController(IMediaStorageManager mediaStorageManager)
        {
            _mediaStorageManager = mediaStorageManager;
        }
        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<Response<DocumentViewModel>> Post(IFormFile document)
        {
            if (document == null)
                throw new MissingParameterException("document");

            var documentVm = await _mediaStorageManager.UploadDocumentAsync(document);

            return new Response<DocumentViewModel>
            {
                Data = documentVm
            };
        }

    }
}
