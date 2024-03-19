using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Models;
//using Emeint.Core.BE.Identity.API.Controllers.Common;
using Emeint.Core.BE.Identity.Domain.Configurations;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Services;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Emeint.Core.BE.Identity.API.Filters
{
    public class IdentityFilter : IResourceFilter
    {
        #region Fields
        private readonly IApplicationVersionRepository _applicationVersionRepository;
        private readonly IIdentityService _identityService;
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<IdentityFilter> _logger;
        #endregion

        #region Constructor
        public IdentityFilter(//IClientStore clientStore, 
            IApplicationVersionRepository applicationVersionRepository,
             IIdentityService identityService, ILogger<IdentityFilter> logger, IProfileRepository profileRepository)
        {
            _applicationVersionRepository = applicationVersionRepository;
            _identityService = identityService;
            _profileRepository = profileRepository;
            _logger = logger;
        }
        #endregion

        private StringValues version, language, platform;
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                if (controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(SkipIdentityFilterAttribute), false).Any())
                {
                    return;
                }
            }

            context.HttpContext.Request.Headers.TryGetValue("Version", out version);
            context.HttpContext.Request.Headers.TryGetValue("Language", out language);
            context.HttpContext.Request.Headers.TryGetValue("Platform", out platform);

            Domain.Enums.Platform devicePlatform = 0;
            var platformName = platform.ToString().ToLower();
            if (platformName != null && platformName == "android")
            {
                devicePlatform = Domain.Enums.Platform.android;
            }
            else if (platformName != null && platformName == "ios")
            {
                devicePlatform = Domain.Enums.Platform.ios;
            }
            else if (platformName != null && platformName == "web")
            {
                devicePlatform = Domain.Enums.Platform.web;
            }
            else if (platformName != null && platformName == "portal")
            {
                devicePlatform = Domain.Enums.Platform.portal;
            }
            else
            {
                throw new MissingParameterException("Platform", platform);
            }

            if (platformName == "android" || platformName == "ios")
            {
                if (string.IsNullOrEmpty(version))
                    throw new MissingParameterException("Version");
            }

            if (!string.IsNullOrEmpty(_identityService.UserId))
            {
                var user = _profileRepository.GetUserById(_identityService.UserId);
                if (user == null)
                    throw new UserNotFoundException();
                bool isDataChanged = false;
                if (user.ApplicationVersion != version || user.Language != language || user.Platform != devicePlatform)
                    isDataChanged = true;
                if (isDataChanged)
                {
                    user.ApplicationVersion = version;
                    user.Language = language;
                    user.Platform = devicePlatform;
                    _profileRepository.SaveEntities();
                }
            }
        }

        public async void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
