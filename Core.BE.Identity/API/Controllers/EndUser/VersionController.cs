using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion;
using Emeint.Core.BE.Identity.API.Controllers.Common;
using Emeint.Core.BE.Identity.Domain.Configurations;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Domain.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emeint.Core.BE.Identity.API.Controllers.EndUser
{
    [Produces("application/json")]
    //[Route("api/identity/version")]
    [Route("api/identity/version/v{version:apiVersion}")]
    [ApiVersion("1.0")]
    //[ApiController]
    public class VersionController : Controller
    {
        #region Fields
        private readonly IIdentityConfigurationManager _configurationManager;
        private readonly IApplicationVersionRepository _applicationVersionRepository;
        //private readonly AppSettings _appSettings;
        private readonly CommonVersionManager _commonVersionManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly string _typeName;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public VersionController(IClientStore clientStore, IApplicationVersionRepository applicationVersionRepository
               , IImageRepository imageRepository, ILogger logger, ILogger<CommonVersionManager> versionLogger
            , IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, IIdentityService identityService
            , IIdentityConfigurationManager configurationManager)
        {
            _applicationVersionRepository = applicationVersionRepository;
            //_appSettings = appSettings.Value;
            _identityService = identityService;
            _userManager = userManager;
            _typeName = GetType().Name;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationManager = configurationManager;

            _commonVersionManager = new CommonVersionManager(applicationVersionRepository, versionLogger, identityService);
        }
        #endregion


        /// <summary>
        /// Used to check user device application version
        /// </summary>
        /// <param name="application_version"></param>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        /// <response code="137 (error_code=137)">No version found</response>
        [HttpGet]
        //[ApiVersion("2.0")]
        [Route("check_application_version")]
        public async Task<Response<CheckApplicationVersionResponseViewModel>> CheckApplicationVersion(string application_version, Domain.Enums.Platform platform)
        {
            return await _commonVersionManager.CheckApplicationVersion(application_version, platform);
        }

        /// <summary>
        /// Deprecated and the Get method should be used instead
        /// </summary>
        /// <param name="application_version"></param>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        /// <response code="137 (error_code=137)">No version found</response>
        [HttpPost]
        //[ApiVersion("1.0",Deprecated = true)]
        [Route("check_application_version")]
        public async Task<Response<CheckApplicationVersionResponseViewModel>> CheckApplicationVersion([FromBody] CheckApplicationVersionViewModel application_version)
        {
            return await _commonVersionManager.CheckApplicationVersion(application_version.Version, application_version.Platform);
        }

        /// <summary>
        /// Used to update or add new user device application version
        /// </summary>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        //[Route("action_on_application_version")]
        //[HttpPost]
        //public BaseResponse ActionOnApplicationVersion([FromBody]ActionOnApplicationVersionViewModel application_version)
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //    _logger.LogInformation($"{_typeName}.{methodName}: Update Application Version api");


        //    if (application_version == null)
        //        throw new MissingParameterException("application_version");

        //    _commonVersionManager.ActionOnApplicationVersion(application_version);


        //    return new Response<bool>() { ErrorCode = (int)ErrorCodes.Success };

        //}
    }
}