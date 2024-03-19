using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Gosour.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Controllers.Common
{
    public class CommonVersionManager
    {
        #region Fields  
        private readonly IApplicationVersionRepository _applicationVersionRepository;
        private readonly ILogger<CommonVersionManager> _logger;
        private readonly string _typeName;
        private readonly IIdentityService _identityService;
        #endregion

        #region Constructor

        public CommonVersionManager(IApplicationVersionRepository applicationVersionRepository, ILogger<CommonVersionManager> logger, IIdentityService identityService)
        {
            _applicationVersionRepository = applicationVersionRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _typeName = GetType().Name;
            _identityService = identityService;
        }
        #endregion

        #region Methods

        public void ActionOnApplicationVersion(ActionOnApplicationVersionViewModel applicationVersion)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var version = _applicationVersionRepository.GetApplicationVersion(applicationVersion.Version, applicationVersion.Platform);
            bool isExist = version != null ? true : false;


            if (applicationVersion.OperationOnApplicationVersion == OperationOnApplicationVersion.Update)
            {
                if (isExist)
                {
                    //if (applicationVersion.ForceUpdated)   //force update
                    //    ForceUpdatePreviousVersions(version.CreationDate, version.Platform);
                    bool isTheOnlyActivePlatformVersion = _applicationVersionRepository.IsTheOnlyActivePlatformVersion(applicationVersion.Platform, applicationVersion.Version);

                    if (isTheOnlyActivePlatformVersion && (applicationVersion.IsBeta || applicationVersion.IsDiscontinued))
                    {
                        throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException("Can't deactivate the only active version!", "لا يمكن تعطيل الاصدار النشط الوحيد!");
                    }
                    else
                    {
                        version.IsBeta = applicationVersion.IsBeta;
                        version.IsDiscontinued = applicationVersion.IsDiscontinued;
                        version.StoreUrl = applicationVersion.StoreUrl;
                        version.ModificationDate = DateTime.UtcNow;
                        version.ModifiedBy = _identityService.DisplayName;

                        _applicationVersionRepository.Update(version);
                        _applicationVersionRepository.SaveEntities();
                    }


                }
                else
                    throw new ApplicationVersionWithCodeNotFoundException();

            }
            else
            {
                if (!isExist)
                {
                    Version maxVersion, validversion;

                    if (!Version.TryParse(applicationVersion.Version, out validversion))
                        throw new InvalidApplicationVersion();

                    var maximumVersion = _applicationVersionRepository.GetMaximumApplicationVersionNumber(applicationVersion.Platform);
                    if (maximumVersion != null)
                    {
                        if (Version.TryParse(maximumVersion, out maxVersion) && maxVersion >= validversion)
                            throw new InvalidApplicationVersion();
                    }



                    version = new ApplicationVersion(applicationVersion.Platform, applicationVersion.Version, applicationVersion.IsDiscontinued, DateTime.UtcNow, null, applicationVersion.IsBeta, applicationVersion.StoreUrl, _identityService.DisplayName);

                    //if (version.ForceUpdated)
                    //    ForceUpdatePreviousVersions(version.CreationDate, version.Platform);

                    _applicationVersionRepository.Add(version);
                    _applicationVersionRepository.SaveEntities();
                }
                else
                    throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException("Application version already exist", "نسخة التطبيق موجودة بالفعل");

            }

        }


        public void DeleteApplicationVersion(string verisonCode)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;


            var version = _applicationVersionRepository.GetApplicationVersionByCode(verisonCode);
            bool isExist = version != null ? true : false;
            if (isExist)
            {
                bool isTheOnlyActivePlatformVersion = _applicationVersionRepository.IsTheOnlyActivePlatformVersion(version.Platform, version.Version);

                if (!isTheOnlyActivePlatformVersion)
                    _applicationVersionRepository.DeleteVersionByCode(version.Code);
                else
                    throw new Emeint.Core.BE.Domain.Exceptions.InvalidOperationException("Can't delete the only active version!", "لا يمكن إلغاء الاصدار النشط الوحيد!");
            }
            else
            {
                _logger.LogInformation($"{_typeName}.{methodName}: Error in  Delete Application Version ");
                throw new ApplicationVersionWithCodeNotFoundException(verisonCode);
            }
        }

        public Response<ApplicationVersionsResponseViewModel> GetApplicationVersions(string version, bool? isBeta, bool? isDiscontinued, Domain.Enums.Platform? platform, VersionSortingViewModel sorting, PaginationVm pagination)
        {
            List<ApplicationVersionViewModel> applicationVersionViewModelList = new List<ApplicationVersionViewModel>();
            var versions = _applicationVersionRepository.GetApplicationVersions(version, isBeta, isDiscontinued, platform).ToList();

            //sorting
            versions = sorting != null ?
                 sorting.SortBy switch
                 {
                     VersionSortBy.CreationDate => sorting.Direction == SortDirection.Asc ? versions.OrderBy(v => v.CreationDate).ToList() : versions.OrderByDescending(v => v.CreationDate).ToList(),
                     VersionSortBy.Version => sorting.Direction == SortDirection.Asc ? versions.OrderBy(v => new Version(v.Version)).ToList() : versions.OrderByDescending(v => new Version(v.Version)).ToList(),
                     _ => versions
                 }
                 : versions;

            //pagination
            var count = versions.Count();
            var versionsPagedList = new PagedList<ApplicationVersion>(count, pagination);
            if (pagination != null)
                versions = versions.Skip(versionsPagedList.SkipCount).Take(versionsPagedList.TakeCount).ToList();

            versionsPagedList.List = versions;
            if (versionsPagedList != null && versionsPagedList.TotalCount > 0)
            {
                foreach (var applicationVersion in versionsPagedList.List)
                {
                    applicationVersionViewModelList.Add(new ApplicationVersionViewModel(applicationVersion));
                }
            }

            return new Response<ApplicationVersionsResponseViewModel>()
            {
                Data = new ApplicationVersionsResponseViewModel()
                {
                    ApplicationVersionsList = applicationVersionViewModelList,
                    Length = count
                }
            };

        }

        public async Task<Response<CheckApplicationVersionResponseViewModel>> CheckApplicationVersion(string version, Domain.Enums.Platform platform)
        {
            if (version == null)
                throw new MissingParameterException("version");

            var applicationVersionStatus = _applicationVersionRepository.CheckApplicationVersion(version, platform);

            if (applicationVersionStatus == null)
                throw new ApplicationVersionWithNumberNotFoundException(version);

            return new Response<CheckApplicationVersionResponseViewModel>()
            {
                Data = applicationVersionStatus
            };
        }

        public void ForceUpdatePreviousVersions(DateTime creationDate, Domain.Enums.Platform platform)
        {
            if (creationDate == null)
                throw new MissingParameterException("creationDate");

            List<ApplicationVersion> previousApplicationVersions = new List<ApplicationVersion>();

            previousApplicationVersions = _applicationVersionRepository.GetAllPreviousVersions(creationDate, platform);

            foreach (var version in previousApplicationVersions)
            {
                version.IsDiscontinued = true;
            }
            _applicationVersionRepository.SaveEntities();
        }

        public Response<ApplicationVersionViewModel> GetApplicationVersionDetails(string applicationVersionCode)
        {
            if (string.IsNullOrEmpty(applicationVersionCode))
                throw new MissingParameterException("applicationVersionCode");

            ApplicationVersionViewModel applicationVersionViewModel = new ApplicationVersionViewModel();

            var versionDetails = _applicationVersionRepository.GetApplicationVersionByCode(applicationVersionCode);

            if (versionDetails != null)
            {
                applicationVersionViewModel.Code = versionDetails.Code;
                applicationVersionViewModel.CreatedBy = versionDetails.CreatedBy;
                applicationVersionViewModel.CreationDate = versionDetails.CreationDate.ToString("yyyyMMddHHmmss");
                applicationVersionViewModel.IsBeta = versionDetails.IsBeta;
                applicationVersionViewModel.IsDiscontinued = versionDetails.IsDiscontinued;
                applicationVersionViewModel.ModificationDate = versionDetails.ModificationDate.HasValue ? versionDetails.ModificationDate.Value.ToString("yyyyMMddHHmmss") : null;
                applicationVersionViewModel.ModifiedBy = versionDetails.ModifiedBy;
                applicationVersionViewModel.Platform = versionDetails.Platform;
                applicationVersionViewModel.StoreURL = versionDetails.StoreUrl;
                applicationVersionViewModel.Version = versionDetails.Version;
            }
            else
                throw new ApplicationVersionWithCodeNotFoundException(applicationVersionCode);

            return new Response<ApplicationVersionViewModel>()
            {
                Data = applicationVersionViewModel
            };
        }


        #endregion
    }
}
