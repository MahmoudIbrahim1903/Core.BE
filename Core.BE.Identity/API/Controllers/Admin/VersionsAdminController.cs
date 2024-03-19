using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion;
using Emeint.Core.BE.Identity.API.Controllers.Common;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers.Admin
{
    [Produces("application/json")]
    [Route("api/identity/admin/versions/v{version:apiVersion}")]
    [ApiVersion("1.0")]
    [ApiController]
    public class VersionsAdminController : ControllerBase
    {
        #region Fields
        private readonly IApplicationVersionRepository _applicationVersionRepository;
        private readonly CommonVersionManager _commonVersionManager;
        private readonly IIdentityService _identityService;
        private readonly string _typeName;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public VersionsAdminController(IApplicationVersionRepository applicationVersionRepository, ILogger<CommonVersionManager> versionLogger, IIdentityService identityService)
        {
            _applicationVersionRepository = applicationVersionRepository;
            _identityService = identityService;
            _typeName = GetType().Name;
            _logger = versionLogger ?? throw new ArgumentNullException(nameof(versionLogger));
            _commonVersionManager = new CommonVersionManager(applicationVersionRepository, versionLogger, identityService);
        }
        #endregion
        /// <summary>
        /// Used to get all application version
        /// </summary>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,VersionsReader, VersionsFullController")]
        public Response<ApplicationVersionsResponseViewModel> GetApplicationVersions(string application_version, bool? is_beta, bool? is_discontinued, Emeint.Core.BE.Identity.Domain.Enums.Platform? platform, VersionSortBy sort_by, SortDirection direction, int page_size, int page_number)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Get All Application Version api");


            //if (application_version_criteria == null)
            //    throw new MissingParameterException("application_version_Criteria");
            //return _commonVersionManager.GetApplicationVersions(application_version_criteria);
            var pagination = (page_number == 0 || page_size == 0) ? null : new PaginationVm { PageSize = page_size, PageNumber = page_number };
            return _commonVersionManager.GetApplicationVersions(application_version, is_beta, is_discontinued, platform, new VersionSortingViewModel { SortBy = sort_by, Direction = direction }, pagination);
        }



        /// <summary>
        /// Used to get application version details
        /// </summary>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        [Route("{application_version_code}")]
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,VersionsReader, VersionsFullController")]
        public Response<ApplicationVersionViewModel> ApplicationVersionDetails([FromRoute] string application_version_code)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Application Version Details api");

            return _commonVersionManager.GetApplicationVersionDetails(application_version_code);

        }


        /// <summary>
        /// Used to update or add new user device application version
        /// </summary>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        //[Route("action_on_application_version")]
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, VersionsFullController")]
        public BaseResponse ActionOnApplicationVersion([FromBody] ActionOnApplicationVersionViewModel application_version)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Update Application Version api");


            if (application_version == null)
                throw new MissingParameterException("application_version");

            _commonVersionManager.ActionOnApplicationVersion(application_version);


            return new Response<bool>() { ErrorCode = (int)ErrorCodes.Success };
        }


        /// <summary>
        /// Used to delete user device application version
        /// </summary>
        /// <response code="HTTP 400 Bad Request">Bad Request</response>
        //[Route("delete_application_version/{application_version_code}")]
        [HttpDelete("{application_version_code}")]
        [Authorize(Roles = "SuperAdmin, VersionsFullController")]
        public BaseResponse Delete(string application_version_code)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: delete Application Version api");


            if (application_version_code == null)
                throw new MissingParameterException("application_version_code");

            _commonVersionManager.DeleteApplicationVersion(application_version_code);


            return new Response<bool>() { ErrorCode = (int)ErrorCodes.Success };
        }
    }
}